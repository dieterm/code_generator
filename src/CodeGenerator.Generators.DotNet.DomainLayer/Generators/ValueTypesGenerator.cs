using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.OnionArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Decorators;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.ProgrammingLanguages;
using CodeGenerator.Generators.DotNet.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.DomainLayer.Generators
{
    public class ValueTypesGenerator : GeneratorBase
    {
        public const string GENERATOR_ID = "DotNet.ValueTypesGenerator";
        private readonly ILogger _logger;
        private Func<DotNetProjectArtifactCreatedEventArgs, Task>? _dotnetprojectcreated_unsubscribe_handler;
        public ValueTypesGenerator(ILogger<ValueTypesGenerator> logger)
        {
            _logger = logger;
        }
        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            _dotnetprojectcreated_unsubscribe_handler = messageBus.Subscribe<DotNetProjectArtifactCreatedEventArgs>(
                async (e) => await OnDotNetProjectCreated(e),
                DomainLayerFilter
            );
        }

        private bool DomainLayerFilter(DotNetProjectArtifactCreatedEventArgs args)
        {
            return args.Result.Workspace.CodeArchitecture is OnionCodeArchitecture onionCodeArchitecture
                && args.Layer == onionCodeArchitecture.DomainLayer.LayerName;
        }

        private async Task OnDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs e)
        {
            if (!(e.Result.Workspace.CodeArchitecture is OnionCodeArchitecture))
            {
                // The code architecture of the workspace is not Onion, this generator is only applicable for Onion architecture, so we skip generation
                _logger.LogWarning("The code architecture of the workspace is not Onion, skipping generation");
            }

            var scope = e.Result.Workspace.FindScope(e.Scope) as OnionScopeArtifact;
            if (scope == null)
            {
                _logger.LogWarning("The scope is not an OnionScopeArtifact, skipping generation");
                return;
            }

            var domainLayer = scope.Domains;
            if (domainLayer == null)
            {
                _logger.LogInformation("No domain layer defined, nothing to generate");
                return;
            }
            var valueTypesFolderArtifact = e.DotNetProjectArtifact.Children.OfType<FolderArtifact>().FirstOrDefault(f => f.FolderName == "ValueTypes");
            if (valueTypesFolderArtifact == null)
            {
                valueTypesFolderArtifact = new FolderArtifact("ValueTypes");
                AddChildArtifactToParent(e.DotNetProjectArtifact, valueTypesFolderArtifact, e.Result);
            }

            
            var domainCount = domainLayer.Count();
            foreach (var domain in domainLayer)
            {
                await GenerateValueTypesForDomain(e, scope, domain, valueTypesFolderArtifact, domainCount);
            }
        }

        private async Task GenerateValueTypesForDomain(DotNetProjectArtifactCreatedEventArgs e, ScopeArtifact scope, DomainArtifact domain, FolderArtifact valueTypesFolderArtifact, int domainCount)
        {
            var valueTypesContainer = domain.ValueTypes;
            if (valueTypesContainer == null)
            {
                _logger.LogInformation("No ValueTypes container defined in the domain layer, nothing to generate");
                return;
            }
            var valueTypes = valueTypesContainer.GetValueTypes().ToList();
            if (valueTypes.Count == 0)
            {
                _logger.LogInformation("No ValueTypes defined in the domain layer, nothing to generate");
                return;
            }
            var domainNamespace = domain.Namespace;
            FolderArtifact domainFolderArtifact = valueTypesFolderArtifact;
            if (domainCount > 1)
            {
                // if there is more than 1 domain, we create a subfolder for each domain inside the Entities folder,
                // if there is only 1 domain, we put the entities directly inside the Entities folder
                domainFolderArtifact = e.DotNetProjectArtifact.Children.OfType<FolderArtifact>().FirstOrDefault(f => f.FolderName == domain.Name);
                if (domainFolderArtifact == null)
                {
                    domainFolderArtifact = new FolderArtifact(domain.Name);
                    AddChildArtifactToParent(valueTypesFolderArtifact, domainFolderArtifact, e.Result);
                }
            }

            foreach (var valueTypeArtifact in valueTypes)
            {
                 await GenerateValueTypeByCodeFile(e, scope, domain, valueTypeArtifact, domainFolderArtifact, domainNamespace);
            }

            // add support for subdomains
            foreach (var subDomain in domain.SubDomains)
            {
                var moreThanOneSubDomain = 2; // always force creation of subfolder for subdomains
                await GenerateValueTypesForDomain(e, scope, subDomain, domainFolderArtifact, moreThanOneSubDomain);
            }
        }

        private Task GenerateValueTypeByCodeFile(DotNetProjectArtifactCreatedEventArgs e, ScopeArtifact scope, DomainArtifact domain, ValueTypeArtifact valueTypeArtifact, FolderArtifact domainFolderArtifact, string domainNamespace)
        {
            var languageId = e.Result.Workspace.DefaultLanguage;
            var language = ProgrammingLanguages.FindById(languageId); // required for file extension
            var codeFileElement = new CodeFileElement(valueTypeArtifact.Name, language);
            var classElement = new ClassElement(valueTypeArtifact.Name) { Documentation = valueTypeArtifact.Description };
            classElement.BaseTypes.Add(new TypeReference("IValueType"));
            codeFileElement.AddNamespace(domainNamespace, classElement);

            foreach (var property in valueTypeArtifact.Properties)
            {
                var mappedType = language.GetMapping(property.DataType);
                var datatype = mappedType != null ? new TypeReference(mappedType.NativeTypeName) { IsNullable = property.IsNullable } : new TypeReference(property.DataType) { IsNullable = property.IsNullable };
                if (property.ValueTypeReference != null)
                {
                    datatype = new TypeReference(property.ValueTypeReference.Name) { IsNullable = property.IsNullable };
                }
                var propertyElement = new PropertyElement(property.Name, datatype)
                {
                    Documentation = (property.Description ?? string.Empty) + (!string.IsNullOrWhiteSpace(property.ExampleValue) ? $"\nFor example: {property.ExampleValue}" : string.Empty),
                };
                classElement.Properties.Add(propertyElement);
            }

            var codeFileArtifact = new CodeFileArtifact(codeFileElement);
            AddChildArtifactToParent(domainFolderArtifact, codeFileArtifact, e.Result);
            return Task.CompletedTask;
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if(_dotnetprojectcreated_unsubscribe_handler != null)
            {
                messageBus.Unsubscribe(_dotnetprojectcreated_unsubscribe_handler);
                _dotnetprojectcreated_unsubscribe_handler = null;
            }
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var id = GENERATOR_ID;
            var name = $"Value Types Generator";
            var description = $"Generates ValueType classes for domain layers";
            var settingsDescription = new GeneratorSettingsDescription(id, name, description);
            return settingsDescription;
        }
    }
}
