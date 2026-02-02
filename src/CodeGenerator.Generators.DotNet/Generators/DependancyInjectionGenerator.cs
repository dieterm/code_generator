using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Generators.DotNet.Events;
using CodeGenerator.Shared;
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

        private readonly DependancyInjectionFrameworkManager _diManager;
        private Action<DotNetProjectArtifactCreatedEventArgs>? _unsubscribe_dotnet_project_created_handler;
        private Action<RequestingPlaceholderContentEventArgs>? _unsubscribe_requesting_dicontainersetup_content_handler;

        public DependancyInjectionGenerator(DependancyInjectionFrameworkManager dependancyInjectionFrameworkManager)
        {
            _diManager = dependancyInjectionFrameworkManager;
        }

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _unsubscribe_dotnet_project_created_handler = messageBus.Subscribe<DotNetProjectArtifactCreatedEventArgs>(
               (e) => OnDotNetProjectCreated(e)
            );
            _unsubscribe_requesting_dicontainersetup_content_handler = messageBus.Subscribe<RequestingPlaceholderContentEventArgs>(OnRequestingDiContainerSetupPlaceholder, DiContainerSetupFilter);
        }

        private void OnRequestingDiContainerSetupPlaceholder(RequestingPlaceholderContentEventArgs args)
        {
            var workspace = args.Result.Workspace;
            if(workspace==null) throw new ApplicationException("Workspace information is missing in the event args.");

            var diFramework = _diManager.GetFrameworkById(workspace.DependencyInjectionFrameworkId);
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
            // create container builder
            // eg. "var services = new ServiceCollection();"
            code.AppendLine("            " + diFramework.GenerateContainerBuilderCreation());

            // add all module registration method calls
            foreach (var scope in workspace.Scopes)
            {
                foreach(var layer in workspace.CodeArchitecture.Layers)
                {
                    var layerName = (layer.CreateLayer(scope.Name) as CodeArchitectureLayerArtifact).Layer;
                    var methodName = diFramework.GenerateModuleRegistrationMethodName(scope.Name, layerName);
                    code.AppendLine("            "+diFramework.GenerateModuleRegistrationMethodCall(methodName));
                }
                foreach(var subScope in scope.SubScopes)
                {
                    foreach (var layer in workspace.CodeArchitecture.Layers)
                    {
                        var layerName = (layer.CreateLayer(scope.Name) as CodeArchitectureLayerArtifact).Layer;
                        var methodName = diFramework.GenerateModuleRegistrationMethodName(subScope.Name, layerName);
                        code.AppendLine("            " + diFramework.GenerateModuleRegistrationMethodCall(methodName));
                    }
                }
            }
            code.AppendLine("            " + diFramework.GenerateBuildContainer());

            args.AddContent(this, code.ToString());
        }

        private bool DiContainerSetupFilter(RequestingPlaceholderContentEventArgs args)
        {
            return args.PlaceHolderName == PLACEHOLDER_DI_CONTAINER_SETUP;
        }

        private void OnDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs e)
        {
            // setup dependancy injection
            var diFramework = _diManager.GetFrameworkById(e.Result.Workspace.DependencyInjectionFrameworkId);
            if (diFramework == null) throw new ApplicationException($"Dependancy injection framework with id '{e.Result.Workspace.DependencyInjectionFrameworkId}' not found.");
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
            var codeFileArtifact = new CodeFileArtifact("DiContainerExtentions", language);

            var moduleRegistrations = new Dictionary<string, IEnumerable<ServiceRegistration>>();
            moduleRegistrations.Add(methodName, new List<ServiceRegistration>
            {
                new ServiceRegistration { ServiceType = new TypeReference("IMyService"), ImplementationType = new TypeReference("MyService"), Lifetime = ServiceLifetime.Scoped },
                new ServiceRegistration { ServiceType = new TypeReference("IMyRepository"), ImplementationType = new TypeReference("MyRepository"), Lifetime = ServiceLifetime.Singleton },
            });
            var diClass = diFramework.GenerateServiceCollectionExtensionsClass("DiContainerExtentions",moduleRegistrations);
            codeFileArtifact.CodeFile.AddNamespace("MyNamespace", diClass);
                
            AddChildArtifactToParent(e.DotNetProjectArtifact, codeFileArtifact, e.Result);
            
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
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var id = nameof(DependancyInjectionGenerator);
            var name = "Dependancy Injection Generator";
            var description = "Generates dependancy injection code for .NET projects based on selected frameworks.";
            var settingsDescription = new GeneratorSettingsDescription(id, name, description);
            return settingsDescription;
        }
    }
}
