using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Events;
using CodeGenerator.Generators.DotNet.Workspace;
using CodeGenerator.Shared;
using CodeGenerator.TemplateEngines.Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators
{
    public class ApplicationControllerGenerator : GeneratorBase
    {
        public const string APPLICATION_CONTROLLER_TEMPLATE_NAME = "ApplicationController.cs";
        public const string IAPPLICATION_SERVICE_TEMPLATE_NAME = "IApplicationService.cs";
        public const string MAIN_VIEW_MODEL_TEMPLATE_NAME = "MainViewModel.cs";

        private Func<DotNetProjectArtifactCreatedEventArgs, Task>? _unsubscribe_dotnet_project_created_handler;
        private Action<DiExtensionsClassArtifactCreatedEventArgs>? _unsubscribe_diextensions_codefile_created_handler;

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _unsubscribe_dotnet_project_created_handler = messageBus.Subscribe<DotNetProjectArtifactCreatedEventArgs>(
               async (e) => await OnDotNetProjectCreated(e), FilterApplicationScopeApplicationLayer          
            );
            _unsubscribe_diextensions_codefile_created_handler = messageBus.Subscribe<DiExtensionsClassArtifactCreatedEventArgs>(OnDiExtensionsCodeFileArtifactCreated, DiExtensionsCodeFileArtifactFilter);
        }

        private bool DiExtensionsCodeFileArtifactFilter(DiExtensionsClassArtifactCreatedEventArgs args)
        {
            return args.Layer == OnionCodeArchitecture.APPLICATION_LAYER &&
                   args.Scope == CodeArchitectureScopes.APPLICATION_SCOPE;
        }

        private void OnDiExtensionsCodeFileArtifactCreated(DiExtensionsClassArtifactCreatedEventArgs args)
        {
            args.DiExtensionsClassArtifact.ServiceRegistrations.Add(new ServiceRegistration
            {
                ServiceType = new TypeReference("ApplicationController"),
                Lifetime = ServiceLifetime.Singleton
            });
            args.DiExtensionsClassArtifact.CodeFile.AddUsing($"{WorkspaceTemplateHelpers.GetApplicationApplicationNamespace()}.Controllers");

            //  MainViewModel
            args.DiExtensionsClassArtifact.ServiceRegistrations.Add(new ServiceRegistration
            {
                ServiceType = new TypeReference("MainViewModel"),
                Lifetime = ServiceLifetime.Singleton
            });
            // using VzwWijzer.Application.Application.ViewModels;
            args.DiExtensionsClassArtifact.CodeFile.AddUsing($"{WorkspaceTemplateHelpers.GetApplicationApplicationNamespace()}.ViewModels");

            // Add Logging
            
            var loggingCode = @"
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug); // Enable Debug level logging
            builder.AddConfiguration(configuration.GetSection(""Logging""));
        });";
            args.DiExtensionsClassArtifact.ServiceRegistrations.Add(new ServiceRegistration { RawRegistrationCode = loggingCode });
            args.DiExtensionsClassArtifact.CodeFile.AddUsing("Microsoft.Extensions.Logging");
        }

        private bool FilterApplicationScopeApplicationLayer(DotNetProjectArtifactCreatedEventArgs args)
        {
            if (!Enabled) 
                return false;

            return args.Layer == OnionCodeArchitecture.APPLICATION_LAYER &&
                   args.Scope == CodeArchitectureScopes.APPLICATION_SCOPE;
        }

        private async Task OnDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs e)
        {
            if (!Enabled)
                return;

            var dotNetProject = e.DotNetProjectArtifact;
            dotNetProject.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Logging);
            dotNetProject.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Logging_Debug);
            dotNetProject.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Logging_Configuration);
            dotNetProject.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Configuration);
            dotNetProject.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Configuration_Json);
            await CreateApplicationController(e, dotNetProject);
            await CreateIApplicationService(e, dotNetProject);
            await CreateMainViewModel(e, dotNetProject);
        }

        private async Task CreateApplicationController(DotNetProjectArtifactCreatedEventArgs e, DotNetProjectArtifact dotNetProject)
        {
            var controllersFolderArtifact = new FolderArtifact("Controllers");
            AddChildArtifactToParent(dotNetProject, controllersFolderArtifact, e.Result);

            // Get the namespace for the Controllers layer
            // eg. MyWorkspace.Application.Application.Controllers
            var controllerNamespace = dotNetProject.GetLayerNamespace("Controllers");

            var appControllerTemplate = GetTemplateInstance<ScribanTemplate, ScribanTemplateInstance>(APPLICATION_CONTROLLER_TEMPLATE_NAME);
            appControllerTemplate.OutputFileName = "ApplicationController.cs";
            appControllerTemplate.SetParameter("Namespace", controllerNamespace);
            appControllerTemplate.SetParameter("ControllerBaseClass", "ControllerBase");
            var output = await appControllerTemplate.RenderAsync(CancellationToken.None);

            AddTemplateOutputToArtifact(output, controllersFolderArtifact, e.Result);
        }

        private async Task CreateIApplicationService(DotNetProjectArtifactCreatedEventArgs e, DotNetProjectArtifact dotNetProject)
        {
            var servicesFolderArtifact = new FolderArtifact("Services");
            AddChildArtifactToParent(dotNetProject, servicesFolderArtifact, e.Result);

            // Get the namespace for the Controllers layer
            // eg. MyWorkspace.Application.Application.Services
            var serviceNamespace = dotNetProject.GetLayerNamespace("Services");

            var appControllerTemplate = GetTemplateInstance<ScribanTemplate, ScribanTemplateInstance>(IAPPLICATION_SERVICE_TEMPLATE_NAME);
            appControllerTemplate.OutputFileName = "IApplicationService.cs";
            appControllerTemplate.SetParameter("Namespace", serviceNamespace);
            

            var output = await appControllerTemplate.RenderAsync(CancellationToken.None);

            AddTemplateOutputToArtifact(output, servicesFolderArtifact, e.Result);
        }

        private async Task CreateMainViewModel(DotNetProjectArtifactCreatedEventArgs e, DotNetProjectArtifact dotNetProject)
        {
            var servicesFolderArtifact = new FolderArtifact("ViewModels");
            AddChildArtifactToParent(dotNetProject, servicesFolderArtifact, e.Result);

            // Get the namespace for the Controllers layer
            // eg. MyWorkspace.Application.Application.Services
            var serviceNamespace = dotNetProject.GetLayerNamespace("ViewModels");

            var appControllerTemplate = GetTemplateInstance<ScribanTemplate, ScribanTemplateInstance>(MAIN_VIEW_MODEL_TEMPLATE_NAME);
            appControllerTemplate.OutputFileName = "MainViewModel.cs";
            appControllerTemplate.SetParameter("Namespace", serviceNamespace);
            appControllerTemplate.SetParameter("BaseClassName", "ViewModelBase");


            var output = await appControllerTemplate.RenderAsync(CancellationToken.None);

            AddTemplateOutputToArtifact(output, servicesFolderArtifact, e.Result);
        }


        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if(_unsubscribe_dotnet_project_created_handler!=null)
            {
                messageBus.Unsubscribe<DotNetProjectArtifactCreatedEventArgs>(_unsubscribe_dotnet_project_created_handler);
                _unsubscribe_dotnet_project_created_handler = null;
            }
            if(_unsubscribe_diextensions_codefile_created_handler!=null)
            {
                messageBus.Unsubscribe<DiExtensionsClassArtifactCreatedEventArgs>(_unsubscribe_diextensions_codefile_created_handler);
                _unsubscribe_diextensions_codefile_created_handler = null;
            }
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var generatorId = nameof(ApplicationControllerGenerator);
            var name = "Application Controller Generator";
            var description = "Generates the application controller for a .NET application.";
            var settingsDescription = new GeneratorSettingsDescription(generatorId, name, description);

            var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();

            var applicationControllerTemplateId = TemplateIdParser.BuildGeneratorTemplateId(generatorId, APPLICATION_CONTROLLER_TEMPLATE_NAME);
            var iApplicationServiceTemplateId = TemplateIdParser.BuildGeneratorTemplateId(generatorId, IAPPLICATION_SERVICE_TEMPLATE_NAME);
            var mainViewModelTemplateId = TemplateIdParser.BuildGeneratorTemplateId(generatorId, MAIN_VIEW_MODEL_TEMPLATE_NAME);

            templateManager.RegisterRequiredTemplate(applicationControllerTemplateId);
            templateManager.RegisterRequiredTemplate(iApplicationServiceTemplateId);
            templateManager.RegisterRequiredTemplate(mainViewModelTemplateId);

            return settingsDescription;
        }
    }
}
