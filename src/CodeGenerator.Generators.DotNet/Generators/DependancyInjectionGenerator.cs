using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Templates.Settings;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
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
    public class DependancyInjectionGenerator : GeneratorBase
    {
        public const string PLACEHOLDER_DI_CONTAINER_SETUP = "DiContainerSetup";
        public const string PLACEHOLDER_DI_CONTAINER_SETUP_USINGS = "DiContainerSetupUsings";
        public const string SERVICE_PROVIDER_HOLDER_TEMPLATE_NAME = "ServiceProviderHolder.cs";
        public const string PARAMETER_SERVICE_PROVIDER_HOLDER_CLASS_NAME = "ServiceProviderHolderClassName";
        
        private readonly DependancyInjectionFrameworkManager _diManager;
        private Func<DotNetProjectArtifactCreatedEventArgs, Task>? _unsubscribe_dotnet_project_created_handler;
        private Action<RequestingPlaceholderContentEventArgs>? _unsubscribe_requesting_dicontainersetup_content_handler;
        private Action<RequestingPlaceholderContentEventArgs>? _unsubscribe_requesting_dicontainersetupusings_content_handler;

        public DependancyInjectionGenerator(DependancyInjectionFrameworkManager dependancyInjectionFrameworkManager)
        {
            _diManager = dependancyInjectionFrameworkManager;
        }

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _unsubscribe_dotnet_project_created_handler = messageBus.Subscribe<DotNetProjectArtifactCreatedEventArgs>(
               async (e) => await OnDotNetProjectCreated(e)
            );
            // PLACEHOLDER_DI_CONTAINER_SETUP
            _unsubscribe_requesting_dicontainersetup_content_handler = messageBus.Subscribe<RequestingPlaceholderContentEventArgs>(OnRequestingDiContainerSetupPlaceholder, DiContainerSetupFilter);
            // PLACEHOLDER_DI_CONTAINER_SETUP_USINGS
            _unsubscribe_requesting_dicontainersetupusings_content_handler += messageBus.Subscribe<RequestingPlaceholderContentEventArgs>(OnRequestingDiContainerSetupUsingsPlaceholder, DiContainerSetupUsingsFilter);
        }

        private bool DiContainerSetupUsingsFilter(RequestingPlaceholderContentEventArgs args)
        {
            return args.PlaceHolderName == PLACEHOLDER_DI_CONTAINER_SETUP_USINGS;
        }

        private void OnRequestingDiContainerSetupUsingsPlaceholder(RequestingPlaceholderContentEventArgs args)
        {
            // enumerate all namespaces of all scopes and subscopes
            var workspace = args.Result.Workspace;
            if (workspace == null) throw new ApplicationException("Workspace information is missing in the event args.");
            var usings = new StringBuilder();
            foreach (var scope in workspace.Scopes)
            {
                AppendModuleRegistrationUsings(workspace, usings, scope);
            }
            args.AddContent(this, usings.ToString());
        }

        private void AppendModuleRegistrationUsings(WorkspaceArtifact workspace, StringBuilder usings, ScopeArtifact scope)
        {
            foreach (var layer in scope.GetLayers())
            {
                usings.AppendLine($"using {(layer as WorkspaceArtifactBase)?.Context?.Namespace};");
            }
            foreach (var subScope in scope.SubScopes)
            {
                AppendModuleRegistrationUsings(workspace, usings, subScope);
            }
        }

        private void OnRequestingDiContainerSetupPlaceholder(RequestingPlaceholderContentEventArgs args)
        {
            var workspace = args.Result.Workspace;
            if(workspace==null) throw new ApplicationException("Workspace information is missing in the event args.");

            var diFramework = _diManager.GetDotNetFrameworkById(workspace.DependencyInjectionFrameworkId);
            if (diFramework == null) throw new ApplicationException($"Dependancy injection framework with id '{workspace.DependencyInjectionFrameworkId}' not found.");
            // TODO: Unify language usage
            ProgrammingLanguage language = ProgrammingLanguage.CSharp;

            var workspaceLanguage = args.Result.Workspace.DefaultLanguage;
            if (workspaceLanguage == CodeGenerator.Domain.ProgrammingLanguages.CSharp.CSharpLanguage.Instance.Id)
            {
                language = ProgrammingLanguage.CSharp;
            }
            else
                throw new NotImplementedException();

            var code = new StringBuilder();
            var indent = "\t\t\t";
            // create container builder
            // eg. "var services = new ServiceCollection();"
            code.AppendLine(indent + diFramework.GenerateContainerBuilderCreation());

            // add all module registration method calls
            foreach (var scope in workspace.Scopes)
            {
                AppendGenerateModuleRegistrationMethodCalls(workspace, diFramework, code, scope, indent);
            }
            code.AppendLine(indent + diFramework.GenerateBuildContainer());

            args.AddContent(this, code.ToString());
        }

        private static void AppendGenerateModuleRegistrationMethodCalls(WorkspaceArtifact workspace, DotNetDependancyInjectionFramework diFramework, StringBuilder code, ScopeArtifact scope, string indent)
        {
            foreach (var layer in scope.GetLayers())
            {
                var methodName = diFramework.GenerateModuleRegistrationMethodName(scope.Name, layer.LayerName);
                code.AppendLine(indent + diFramework.GenerateModuleRegistrationMethodCall(methodName));
            }
            foreach (var subScope in scope.SubScopes)
            {
                AppendGenerateModuleRegistrationMethodCalls(workspace, diFramework, code, subScope, indent);
            }
        }

        private bool DiContainerSetupFilter(RequestingPlaceholderContentEventArgs args)
        {
            return args.PlaceHolderName == PLACEHOLDER_DI_CONTAINER_SETUP;
        }

        private async Task OnDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs e)
        {
            var dotNetProject = e.DotNetProjectArtifact;
            
            var projectFolderArtifact = dotNetProject.Parent as FolderArtifact;
            var layerArtifact = projectFolderArtifact?.GetDecoratorOfType<LayerArtifactRefDecorator>()?.LayerArtifact;//.Parent as ScopeArtifact;
            var layerNamespace = (layerArtifact as WorkspaceArtifactBase).Context?.Namespace;

            // setup dependancy injection
            var diFramework = _diManager.GetDotNetFrameworkById(e.Result.Workspace.DependencyInjectionFrameworkId);
            if (diFramework == null) throw new ApplicationException($"Dependancy injection framework with id '{e.Result.Workspace.DependencyInjectionFrameworkId}' not found.");
            
            // add required nuget packages to the project
            diFramework.GetRequiredNuGetPackages().ToList().ForEach(pkg => dotNetProject.AddNuGetPackage(pkg));
            diFramework.GetOptionalNuGetPackages().ToList().ForEach(pkg => dotNetProject.AddNuGetPackage(pkg));
            // TODO: Unify language usage
            ProgrammingLanguage language = ProgrammingLanguage.CSharp;
            var workspaceLanguage = e.Result.Workspace.DefaultLanguage;
            if (workspaceLanguage == CodeGenerator.Domain.ProgrammingLanguages.CSharp.CSharpLanguage.Instance.Id)
            {
                language = ProgrammingLanguage.CSharp;
            }
            else
                throw new NotImplementedException();

            // eg. "AddApplicationDomainServices"
            var methodName = diFramework.GenerateModuleRegistrationMethodName(e.Scope, e.Layer);
            // register types
            var codeFileArtifact = new DiExtensionsClassArtifact(language);
            // publish event for DiExtensionsClassArtifact creation so that other generators can add content to it before it's added to the project
            MessageBus?.Publish(new DiExtensionsClassArtifactCreatedEventArgs(codeFileArtifact, e.Layer, e.Scope, e.Result));
   
            var moduleRegistrations = new Dictionary<string, IEnumerable<ServiceRegistration>>();
            moduleRegistrations.Add(methodName, codeFileArtifact.ServiceRegistrations);
            var diClass = diFramework.GenerateServiceCollectionExtensionsClass(DiExtensionsClassArtifact.DI_CONTAINER_EXTENSIONS_CLASS_NAME, moduleRegistrations);
            codeFileArtifact.CodeFile.AddNamespace(layerNamespace, diClass);
            codeFileArtifact.CodeFile.Usings.AddRange(diFramework.GetExtensionMethodUsings().Select(x => new UsingElement(x)));
            codeFileArtifact.CodeFile.Usings.AddRange(diFramework.GetRequiredUsings().Select(x => new UsingElement(x)));
            try
            {

           
            foreach(var module in moduleRegistrations)
            {
                foreach(var registration in module.Value.ToArray())
                {
                    if(registration.ServiceType?.Namespace!=null)
                    {
                        codeFileArtifact.CodeFile.AddUsing(registration.ServiceType?.Namespace);
                    }
                    if(registration.ImplementationType?.Namespace!=null)
                    {
                        codeFileArtifact.CodeFile.AddUsing(registration.ImplementationType?.Namespace);
                    }
                    codeFileArtifact.ServiceRegistrations.Add(registration);
                }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            AddChildArtifactToParent(e.DotNetProjectArtifact, codeFileArtifact, e.Result);

            // Create ServiceProviderHolder class in Shared.Domain project
            if (e.Scope == CodeArchitectureScopes.SHARED_SCOPE && e.Layer == OnionCodeArchitecture.DOMAIN_LAYER)
            { 
                var serviceProviderHolderTemplateId = TemplateIdParser.BuildGeneratorTemplateId(
                    nameof(DependancyInjectionGenerator),
                    SERVICE_PROVIDER_HOLDER_TEMPLATE_NAME,
                    diFramework.Id
                );
                var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
                var serviceProviderHolderTemplate = templateManager.GetTemplateById(serviceProviderHolderTemplateId) as ScribanTemplate;
                if (serviceProviderHolderTemplate == null) throw new ApplicationException($"ServiceProviderHolder template with id '{serviceProviderHolderTemplateId}' not found.");
                var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
                var templateEngine = templateEngineManager.GetTemplateEnginesForTemplate(serviceProviderHolderTemplate).FirstOrDefault();
                if (templateEngine == null) throw new ApplicationException($"No template engine found for template id '{serviceProviderHolderTemplateId}'.");
                var scribanTemplateInstance = templateEngine.CreateTemplateInstance(serviceProviderHolderTemplate) as ScribanTemplateInstance;
                if (scribanTemplateInstance == null) throw new ApplicationException("Template instance is not of type ScribanTemplateInstance.");
            
                var settings = GetSettings();
                var className = settings.GetParameter<string>(PARAMETER_SERVICE_PROVIDER_HOLDER_CLASS_NAME, SettingsDescription.ParameterDefinitions.Single( p=> p.Name== PARAMETER_SERVICE_PROVIDER_HOLDER_CLASS_NAME).DefaultValue as string);
            
                scribanTemplateInstance.OutputFileName = $"{className}.cs";
            
                // set parameters
                scribanTemplateInstance.SetParameter("Namespace", layerNamespace);
                scribanTemplateInstance.SetParameter("ClassName", className);

                // render template
                var templateOutput = await templateEngine.RenderAsync(scribanTemplateInstance);
                if (templateOutput.Succeeded)
                {
                    foreach(var artifact in templateOutput.Artifacts)
                    {
                        AddChildArtifactToParent(e.DotNetProjectArtifact, artifact, e.Result);
                    }
                } else
                {
                    e.Result.Errors.AddRange(templateOutput.Errors);
                }
            }
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if(_unsubscribe_dotnet_project_created_handler!=null)
            {
                messageBus.Unsubscribe(_unsubscribe_dotnet_project_created_handler);
                _unsubscribe_dotnet_project_created_handler = null;
            }
            if(_unsubscribe_requesting_dicontainersetup_content_handler != null)
            {
                messageBus.Unsubscribe(_unsubscribe_requesting_dicontainersetup_content_handler);
                _unsubscribe_requesting_dicontainersetup_content_handler = null;
            }
            if (_unsubscribe_requesting_dicontainersetupusings_content_handler != null)
            {
                messageBus.Unsubscribe(_unsubscribe_requesting_dicontainersetupusings_content_handler);
                _unsubscribe_requesting_dicontainersetupusings_content_handler = null;
            }
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var generatorId = nameof(DependancyInjectionGenerator);
            var name = "Dependancy Injection Generator";
            var description = "Generates dependancy injection code for .NET projects based on selected frameworks.";
            var settingsDescription = new GeneratorSettingsDescription(generatorId, name, description);

            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition { 
                Name= PARAMETER_SERVICE_PROVIDER_HOLDER_CLASS_NAME, 
                DefaultValue="ServiceProviderHolder", 
                Required = true ,
                Type = ParameterDefinitionTypes.String
            });

            // since ConfigureSettingsDescription() is called in construction time, we cannot use _diManager here
            // so instead, we will get it from the ServiceProviderHolder
            var diManager = ServiceProviderHolder.GetRequiredService<DependancyInjectionFrameworkManager>();
            var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
            foreach (var depFramework in diManager.GetDotNetFrameworks())
            {
                var templateId = TemplateIdParser.BuildGeneratorTemplateId(generatorId, SERVICE_PROVIDER_HOLDER_TEMPLATE_NAME, depFramework.Id);
                templateManager.RegisterRequiredTemplate(templateId);
            }
            
            
            return settingsDescription;
        }
    }
}
