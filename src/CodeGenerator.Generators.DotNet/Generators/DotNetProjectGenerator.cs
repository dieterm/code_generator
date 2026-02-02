using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Generators;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Events;

namespace CodeGenerator.Generators.DotNet.Generators
{
    public class DotNetProjectGenerator : GeneratorBase
    {
        private Func<CreatedArtifactEventArgs, Task>? _unsubscribe_created_artifact_layerscope_handler;
  
        public virtual bool ScopeFolderArtifactFilter(CreatedArtifactEventArgs e)
        {
            return e.Artifact is FolderArtifact folderArtifact && folderArtifact.HasDecorator<ScopeArtifactRefDecorator>();
        }

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            // Use async Subscribe variant for proper async handling
            _unsubscribe_created_artifact_layerscope_handler = messageBus.Subscribe<CreatedArtifactEventArgs>(
                async (e) => await OnScopeFolderCreatedAsync(e), 
                ScopeFolderArtifactFilter
            );
           
        }

        public Task OnScopeFolderCreatedAsync(CreatedArtifactEventArgs args)
        {
            if (!Enabled)
                return Task.CompletedTask;
            var workspaceArtifact = args.Result.Workspace;
            var scopeFolderArtifact = args.Artifact as FolderArtifact;
            if (scopeFolderArtifact == null) throw new ArgumentException("Artifact is not a FolderArtifact");
            var scopeDecorator = scopeFolderArtifact.GetDecoratorOfType<ScopeArtifactRefDecorator>();
            if( scopeDecorator == null) throw new ArgumentException("FolderArtifact does not have a ScopeArtifactRefDecorator");
            if(scopeDecorator.ScopeArtifact==null) throw new ArgumentException("ScopeArtifactRefDecorator does not have a ScopeArtifact");
            var scopeArtifact = scopeDecorator.ScopeArtifact;
            
            var layers = new List<IArtifact>()
            {
                scopeArtifact.Applications,
                scopeArtifact.Domains,
                scopeArtifact.Infrastructure,
                scopeArtifact.Presentations,
            };

            var language = DotNetLanguages.AllLanguages.FirstOrDefault(l => l.Id==workspaceArtifact.DefaultLanguage);
            var targetFramework = TargetFrameworks.AllFrameworks.FirstOrDefault(f => f.Id == workspaceArtifact.DefaultTargetFramework);
            var winformsTargetFramework = TargetFrameworks.AllFrameworks.FirstOrDefault(f => f.Id == workspaceArtifact.DefaultTargetFramework+"_windows");
            if (targetFramework==null) throw new ArgumentException($"Target framework '{workspaceArtifact.DefaultTargetFramework}' not found.");

            foreach (var layer in layers)
            {
                var projectType = (layer== scopeArtifact.Presentations) ? DotNetProjectType.WinFormsLib : DotNetProjectType.ClassLib;
                var folderName = scopeArtifact.Namespace+"."+(layer as ILayerArtifact)!.LayerName;
                var layerFolderArtifact = new FolderArtifact(folderName);
                layerFolderArtifact.AddDecorator(new LayerArtifactRefDecorator(nameof(LayerArtifactRefDecorator), layer as ILayerArtifact));
                AddChildArtifactToParent(scopeFolderArtifact, layerFolderArtifact, args.Result);

                var dotNetProjectArtifact = new DotNetProjectArtifact(folderName, language, projectType, (layer == scopeArtifact.Presentations) ? winformsTargetFramework : targetFramework);
                dotNetProjectArtifact.SolutionSubFolder = scopeArtifact.GetSolutionSubFolder();
                AddChildArtifactToParent(layerFolderArtifact, dotNetProjectArtifact, args.Result);
                PublishDotNetProjectCreated(dotNetProjectArtifact, args.Result, (layer as ILayerArtifact)!.LayerName, scopeArtifact.Name);
            }
            return Task.CompletedTask;
        }

        private void PublishDotNetProjectCreated(DotNetProjectArtifact dotNetProjectArtifact, GenerationResult result, string layer, string scope)
        {
            MessageBus?.Publish(new DotNetProjectArtifactCreatedEventArgs(result, dotNetProjectArtifact, layer, scope));
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if (_unsubscribe_created_artifact_layerscope_handler != null)
                messageBus.Unsubscribe<CreatedArtifactEventArgs>(_unsubscribe_created_artifact_layerscope_handler);
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var id = nameof(DotNetProjectGenerator);
            var name = "DotNet Project Generator";
            var description = "Generates .NET project artifacts for each layer in a scoped folder.";
            return new GeneratorSettingsDescription(id, name, description);
        }
    }
}
