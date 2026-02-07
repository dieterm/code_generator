using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Events;
using CodeGenerator.Shared;
using CodeGenerator.TemplateEngines.Folder;
using CodeGenerator.TemplateEngines.T4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.SharedScope.Generators
{
    public class DotNetSharedScopeGenerator : GeneratorBase
    {
        private Func<DotNetProjectArtifactCreatedEventArgs, Task>? _unsubscribe_handler;

        public DotNetSharedScopeGenerator()
            : base()
        {
            
        }

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            // Use async Subscribe variant for proper async handling
            _unsubscribe_handler = messageBus.Subscribe<DotNetProjectArtifactCreatedEventArgs>(
                async (e) => await OnSharedScopeDotNetProjectCreated(e),
                SharedScopeDotNetProjectFilter
            );
        }

        private bool SharedScopeDotNetProjectFilter(DotNetProjectArtifactCreatedEventArgs args)
        {
            return Enabled &&
                   args.Scope == CodeArchitectureScopes.SHARED_SCOPE;
        }

        private async Task OnSharedScopeDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs args)
        {
            if(!Enabled)
                return;
            var projectArtifact = args.DotNetProjectArtifact;
            projectArtifact.AddNuGetPackage(NuGetPackages.System_Drawing_Common);
            var templateId = TemplateIdParser.BuildGeneratorTemplateId(Id, args.Layer, args.Result.Workspace.CodeArchitecture!.Id);
            var settings = GetSettings();
            var template = settings.Templates.FirstOrDefault(t => t.TemplateId == templateId)!;
            
            var folderTemplate = new FolderTemplate(templateId);
            var folderTemplateInstance = new FolderTemplateInstance(folderTemplate);

            // Set parameters for the template instance based on the event args

            // for now, we asume the root namespace is the same as the project name.
            // but this could be made more flexible in the future if needed (eg allow user to specify in settings or determine based on solution structure)
            folderTemplateInstance.SetParameter("RootNamespace", projectArtifact.Name);

            // set common template handler to handle namespace generation based on folder structure
            folderTemplateInstance.TemplateHandler = new TemplateHandler
            {
                PrepareTemplateInstance = (instance) =>
                {
                    // the folder template engine will set the "FolderNamespace" parameter based on the folder structure.
                    // we can use this to build the full namespace for each file.
                    var folderNamespace = instance.Parameters[FolderTemplateEngine.TEMPLATE_PARAMETER_FOLDER_NAMESPACE] as string;
                    var rootNamespace = instance.Parameters["RootNamespace"] as string;
                    if (!string.IsNullOrEmpty(folderNamespace))
                    {
                        instance.SetParameter("Namespace", $"{rootNamespace}.{folderNamespace}");
                    }
                    else
                    {
                        instance.SetParameter("Namespace", rootNamespace);
                    }

                }
            };
            var renderResult = await folderTemplateInstance.RenderAsync(CancellationToken.None);
            if (renderResult.Succeeded)
            {
                foreach (var artifact in renderResult.Artifacts)
                {
                    AddChildArtifactToParent(projectArtifact, artifact, args.Result);
                }
            }
            else
            {
                // Log errors
                foreach (var error in renderResult.Errors)
                {
                    args.Result.Errors.Add($"Error rendering template '{templateId}': {error}");
                }
            }
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if (_unsubscribe_handler != null)
            {
                messageBus.Unsubscribe(_unsubscribe_handler);
                _unsubscribe_handler = null;
            }
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            return DotNetSharedScopeGeneratorSettings.CreateDescription(this);
        }
    }
}
