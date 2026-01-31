using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Events.Application;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.CodeArchitectureLayers.ApplicationLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.DomainLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.InfrastructureLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.PresentationLayer;
using CodeGenerator.Generators.DotNet.Events;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Models;
using CodeGenerator.TemplateEngines.DotNetProject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators
{
    public abstract class DotNetProjectGenerator<TLayer> : GeneratorBase where TLayer : CodeArchitectureLayerArtifact
    {
        private Func<CreatedArtifactEventArgs, Task>? _unsubscribe_created_artifact_layerscope_handler;
       
        public ILogger Logger { get; }
        public string Layer { get; }
        public string Scope { get; }
        
        protected DotNetProjectGenerator(string layer, string scope, ILogger logger)
        {
            Layer = layer;
            Scope = scope;
            
            Logger = logger;
            SettingsDescription = ConfigureSettingsDescription();
        }
        public virtual bool LayerArtifactFilter(CreatedArtifactEventArgs e)
        {
            return e.Artifact is TLayer a && a.Scope == Scope;
        }

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            // Use async Subscribe variant for proper async handling
            _unsubscribe_created_artifact_layerscope_handler = messageBus.Subscribe<CreatedArtifactEventArgs>(
                async (e) => await OnLayerScopeCreatedAsync(e), 
                LayerArtifactFilter
            );
           
        }

        protected virtual async Task<DotNetProjectArtifact> OnLayerScopeCreatedAsync(CreatedArtifactEventArgs args)
        {
            if (!Enabled)
                return null;

            var appLayerArtifact = args.Artifact as TLayer;
            if (appLayerArtifact == null) throw new ArgumentException("Artifact is not an ApplicationLayerArtifact");

            // GET SETTINGS
            var settings = new DotNetProjectGeneratorSettings<TLayer>(base.GetSettings(), Layer);

            var language = DotNetLanguages.GetByCommandLineArgument(settings.Language);
            var targetFramework = settings.TargetFramework;
            var projectType = settings.ProjectType;
            
            var workspaceContextProvider = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
            var workspace = workspaceContextProvider.CurrentWorkspace!;

            var projectNameTemplate = new ParameterizedString(settings.ProjectNamePattern);
            var paramters = new Dictionary<string, string>
            {
                { DotNetProjectGeneratorSettings<TLayer>.ProjectNamePattern_LayerParameter, appLayerArtifact.Layer },
                { DotNetProjectGeneratorSettings<TLayer>.ProjectNamePattern_ScopeParameter, appLayerArtifact.Scope },
                { DotNetProjectGeneratorSettings<TLayer>.ProjectNamePattern_LanguageParameter, language.DotNetCommandLineArgument },
                { DotNetProjectGeneratorSettings<TLayer>.ProjectNamePattern_WorkspaceNamespaceParameter, workspace.RootNamespace ?? WorkspaceSettings.Instance.RootNamespace }
            };
            // eg. $"{Layer}.{Scope}" -> "Application.Shared"
            var projectName = projectNameTemplate.GetOutput(paramters);
            var dotNetProjectArtifact = new DotNetProjectArtifact(projectName, language, projectType, targetFramework);

            AddChildArtifactToParent(appLayerArtifact, dotNetProjectArtifact, args.Result);
            PublishDotNetProjectCreated(dotNetProjectArtifact, args.Result, appLayerArtifact.Layer, appLayerArtifact.Scope);
            //var dotNetProjectTemplate = new DotNetProjectTemplate(projectType, language, targetFramework);
            //var dotNetProjectTemplateInstance = new DotNetProjectTemplateInstance(dotNetProjectTemplate, projectName);
            //foreach(var nugetPackage in dotNetProjectArtifact.NuGetPackages)
            //{
            //    dotNetProjectTemplateInstance.Packages.Add(nugetPackage);
            //}
            //foreach(var projectReference in dotNetProjectArtifact.ProjectReferences)
            //{
            //    dotNetProjectTemplateInstance.ProjectReferences.Add(projectReference);
            //}
            //if (!args.Result.PreviewOnly)
            //{
            //    dotNetProjectTemplateInstance.OutputDirectory = dotNetProjectArtifact.FindAncesterOfType<FolderArtifact>()?.FullPath;
            //}
            //var messageBus = ServiceProviderHolder.GetRequiredService<ApplicationMessageBus>();

            //messageBus.Publish(new ReportTaskProgressEvent($"Generating {projectName} .NET project...", null));

            // Use await for proper async handling - no deadlock risk
            //var result = await DotNetProjectTemplateEngine.RenderAsync(dotNetProjectTemplateInstance, CancellationToken.None);

            //messageBus.Publish(new ReportTaskProgressEvent($"Finished generating {projectName} .NET project.", null));
            //if (result.Succeeded)
            //{
            //    // for now, only add the project file as child artifact
            //    var projectFileArtifact = result.Artifacts.OfType<FileArtifact>().FirstOrDefault(f => f.FileName.EndsWith(language.ProjectFileExtension));
                
            //    AddChildArtifactToParent(dotNetProjectArtifact, projectFileArtifact, args.Result);
            //}
            //else
            //{
            //    foreach(var error in result.Errors)
            //    {
            //        args.Result.Errors.Add(error);
            //    }
            //}
            return dotNetProjectArtifact;
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
            return DotNetProjectGeneratorSettings<TLayer>.CreateDescription(this);
        }
    }
}
