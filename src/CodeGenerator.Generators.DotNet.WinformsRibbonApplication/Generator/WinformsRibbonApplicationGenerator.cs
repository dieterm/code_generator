using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Events;
using CodeGenerator.Shared;
using CodeGenerator.TemplateEngines.Folder;
using CodeGenerator.TemplateEngines.Scriban;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication
{
    public class WinformsRibbonApplicationGenerator : GeneratorBase
    {
        private Func<DotNetProjectArtifactCreatedEventArgs, Task>? _unsubscribe_handler;
        private Action<DiExtensionsClassArtifactCreatedEventArgs>? _unsubscribe_diextensions_codefile_created_handler;

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            // Use async Subscribe variant for proper async handling
            _unsubscribe_handler = messageBus.Subscribe<DotNetProjectArtifactCreatedEventArgs>(
                async (e) => await OnPresentationDotNetProjectCreated(e),
                PresentationDotNetProjectFilter
            );
            _unsubscribe_diextensions_codefile_created_handler = messageBus.Subscribe<DiExtensionsClassArtifactCreatedEventArgs>(
                OnDiExtensionsCodeFileArtifactCreated, 
                DiExtensionsCodeFileArtifactFilter
            );
        }

        private bool DiExtensionsCodeFileArtifactFilter(DiExtensionsClassArtifactCreatedEventArgs args)
        {
            return Enabled &&
                   args.Layer == OnionCodeArchitecture.PRESENTATION_LAYER &&
                   args.Scope == CodeArchitectureScopes.APPLICATION_SCOPE;
        }

        private void OnDiExtensionsCodeFileArtifactCreated(DiExtensionsClassArtifactCreatedEventArgs args)
        {
            args.DiExtensionsClassArtifact.ServiceRegistrations.Add(new ServiceRegistration{
                ServiceType = new TypeReference("IApplicationService"),
                ImplementationType = new TypeReference("ApplicationService"),
                Lifetime = ServiceLifetime.Singleton
            });
            args.DiExtensionsClassArtifact.CodeFile.AddUsing($"{WorkspaceTemplateHelpers.GetApplicationApplicationNamespace()}.Services");
            args.DiExtensionsClassArtifact.CodeFile.AddUsing($"{WorkspaceTemplateHelpers.GetApplicationPresentationNamespace()}.Services");
            args.DiExtensionsClassArtifact.ServiceRegistrations.Add(new ServiceRegistration
            {
                ServiceType = new TypeReference("MainView"),
                Lifetime = ServiceLifetime.Transient
            });
            // using VzwWijzer.Application.Presentation.Views;
            args.DiExtensionsClassArtifact.CodeFile.AddUsing($"{WorkspaceTemplateHelpers.GetApplicationPresentationNamespace()}.Views");
        }

        private bool PresentationDotNetProjectFilter(DotNetProjectArtifactCreatedEventArgs args)
        {
            if(!Enabled)
                return false;

            var projectArtifact = args.DotNetProjectArtifact;

            if(projectArtifact.Parent is FolderArtifact folderArtifact && folderArtifact.HasDecorator<LayerArtifactRefDecorator>())
            {
                if(args.Layer == OnionCodeArchitecture.PRESENTATION_LAYER &&
                    args.Scope == CodeArchitectureScopes.APPLICATION_SCOPE)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task OnPresentationDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs e)
        {
            if (!Enabled)
                return;

            var projectArtifact = e.DotNetProjectArtifact;
      
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_Edit_Windows);
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_Grouping_Base);
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_Office2019Theme_WinForms);
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_SfListView_WinForms);
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_Shared_Base);
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_Tools_Windows);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_EntityFrameworkCore);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_EntityFrameworkCore_Relational);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Configuration);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Configuration_Json);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_DependencyInjection);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Hosting);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Logging);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Logging_Debug);

            projectArtifact.AddEmbeddedResource(new EmbeddedResource("Resources\\LucideIcons_#000000.resx"));
            projectArtifact.AddEmbeddedResource(new EmbeddedResource("Resources\\LucideIcons_#ffffff.resx"));

            var settings = GetSettings();
            var templateId = TemplateIdParser.BuildGeneratorTemplateId(this.SettingsDescription.Id, WinformsRibbonApplicationGeneratorSettings.TEMPLATE_ID);
            var template = settings.Templates.FirstOrDefault(t => t.TemplateId == templateId);
            var templateFolder = template?.TemplateFilePath;
            
            if (Directory.Exists(templateFolder))
            {
                var templateManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
                var folderTemplate = new FolderTemplate(templateId);
                var templateInstance = new FolderTemplateInstance(folderTemplate);
                // for now, we asume the root namespace is the same as the project name.
                // but this could be made more flexible in the future if needed (eg allow user to specify in settings or determine based on solution structure)
                templateInstance.SetParameter("RootNamespace", projectArtifact.Name);
                // set common template handler to handle namespace generation based on folder structure
                templateInstance.TemplateHandler = new TemplateHandler {
                    PrepareTemplateInstance = (instance) =>
                    {
                        // the folder template engine will set the "FolderNamespace" parameter based on the folder structure.
                        // we can use this to build the full namespace for each file.
                        var folderNamespace = instance.Parameters[FolderTemplateEngine.TEMPLATE_PARAMETER_FOLDER_NAMESPACE] as string;
                        var rootNamespace = instance.Parameters["RootNamespace"] as string;
                        if (!string.IsNullOrEmpty(folderNamespace))
                        {
                            instance.SetParameter("Namespace", $"{rootNamespace}.{folderNamespace}");
                        } else
                        {
                            instance.SetParameter("Namespace", rootNamespace);
                        }

                    }
                };
                var renderResult = await templateInstance.RenderAsync(CancellationToken.None);
                if (renderResult.Succeeded)
                {
                    foreach (var artifact in renderResult.Artifacts)
                    {
                        AddChildArtifactToParent(projectArtifact, artifact, e.Result);
                    }
                }
                else
                {
                    // Log errors
                    foreach (var error in renderResult.Errors)
                    {
                        e.Result.Errors.Add($"Error rendering template '{templateFolder}': {error}");
                    }
                }
            }
        }


        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if(_unsubscribe_handler!=null)
                messageBus.Unsubscribe(_unsubscribe_handler!);
            if(_unsubscribe_diextensions_codefile_created_handler!=null)
                messageBus.Unsubscribe(_unsubscribe_diextensions_codefile_created_handler!);
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            return WinformsRibbonApplicationGeneratorSettings.CreateDescription(this);
        }
    }
}
