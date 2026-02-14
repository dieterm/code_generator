using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Templates.Settings;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Decorators;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.ProgrammingLanguages;
using CodeGenerator.Generators.DotNet.Events;
using CodeGenerator.TemplateEngines.Scriban;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.DomainLayer.Generators
{
    public class EntityTemplateGenerator : GeneratorBase
    {
        public const string GENERATOR_ID = nameof(EntityTemplateGenerator);
        public const string ENTITY_TEMPLATE_NAME = "EntityTemplate";
        private Func<DotNetProjectArtifactCreatedEventArgs, Task>? _dotnetprojectcreated_unsubscribe_handler;

        public static string ENTITY_TEMPLATE_ID { get { return TemplateIdParser.BuildGeneratorTemplateId(GENERATOR_ID, ENTITY_TEMPLATE_NAME); } }
        private readonly TemplateManager _templateManager;
        private readonly TemplateEngineManager _templateEngineManager;
        private readonly ILogger _logger;
        public EntityTemplateGenerator(TemplateManager templateManager, TemplateEngineManager templateEngineManager, ILogger<EntityTemplateGenerator> logger)
        {
            _templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
            _templateEngineManager = templateEngineManager ?? throw new ArgumentNullException(nameof(templateEngineManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            if(!Enabled) return;
            if(!(e.Result.Workspace.CodeArchitecture is OnionCodeArchitecture))
            {
                // The code architecture of the workspace is not Onion, this generator is only applicable for Onion architecture, so we skip generation
                _logger.LogWarning("The code architecture of the workspace is not Onion, skipping generation");
            }

            var scope = e.Result.Workspace.FindScope(e.Scope);
            
            var domainLayer = scope.Domains;
            if (domainLayer == null) {
                _logger.LogInformation("No domain layer defined, nothing to generate");
                return;
            }
            var rootNamespace = e.DotNetProjectArtifact.Name;
            var entitiesFolderArtifact = e.DotNetProjectArtifact.Children.OfType<FolderArtifact>().FirstOrDefault(f => f.FolderName == "Entities");
            if (entitiesFolderArtifact == null)
            {
                entitiesFolderArtifact = new FolderArtifact("Entities");
                AddChildArtifactToParent(e.DotNetProjectArtifact, entitiesFolderArtifact, e.Result);
            }
            var entitiesRootNamespace = $"{rootNamespace}.Entities";
            var domainCount = domainLayer.Count();
            foreach (var domain in domainLayer)
            {
                await GenerateEntitiesForDomain(e, scope, domain, entitiesFolderArtifact, domainCount, entitiesRootNamespace);
            }
            
        }

        private async Task GenerateEntitiesForDomain(DotNetProjectArtifactCreatedEventArgs e, ScopeArtifact scope, DomainArtifact domain, FolderArtifact entitiesFolderArtifact, int domainCount, string entitiesRootNamespace)
        {
            var entitiesContainer = domain.Entities;
            if (entitiesContainer == null)
            {
                _logger.LogInformation("No entities container defined in the domain layer, nothing to generate");
                return;
            }
            var entities = entitiesContainer.GetEntities().ToList();
            if (entities.Count == 0)
            {
                _logger.LogInformation("No entities defined in the domain layer, nothing to generate");
                return;
            }
            var domainNamespace = domain.Namespace;
            FolderArtifact domainFolderArtifact = entitiesFolderArtifact;
            if (domainCount> 1)
            {
                // if there is more than 1 domain, we create a subfolder for each domain inside the Entities folder,
                // if there is only 1 domain, we put the entities directly inside the Entities folder
                domainFolderArtifact = e.DotNetProjectArtifact.Children.OfType<FolderArtifact>().FirstOrDefault(f => f.FolderName == domain.Name);
                if (domainFolderArtifact == null)
                {
                    domainFolderArtifact = new FolderArtifact(domain.Name);
                    AddChildArtifactToParent(entitiesFolderArtifact, domainFolderArtifact, e.Result);
                }
            }

            foreach (var entityArtifact in entities)
            {
                var templateDecorator = entityArtifact.GetDecoratorOfType<EntityTemplateArtifactDecorator>();
                templateDecorator = null; // for testing
                if (templateDecorator != null && !string.IsNullOrWhiteSpace(templateDecorator.TemplateId))
                {
                    await GenerateEntityByTemplateId(e, scope, domain, entityArtifact, domainFolderArtifact, domainNamespace);
                } 
                else
                {
                    await GenerateEntityByCodeFile(e, scope, domain, entityArtifact, domainFolderArtifact, domainNamespace);
                }
            }

            // add support for subdomains
            foreach(var subDomain in domain.SubDomains)
            {
                var moreThanOneSubDomain = 2; // always force creation of subfolder for subdomains
                await GenerateEntitiesForDomain(e, scope, subDomain, domainFolderArtifact, moreThanOneSubDomain, entitiesRootNamespace);
            }
        }

        private Task GenerateEntityByCodeFile(DotNetProjectArtifactCreatedEventArgs e, ScopeArtifact scope, DomainArtifact domain, EntityArtifact entityArtifact, FolderArtifact domainFolderArtifact, string domainNamespace)
        {
            var languageId = e.Result.Workspace.DefaultLanguage;
            var language = ProgrammingLanguages.FindById(languageId); // required for file extension
            var codeFileElement = new CodeFileElement(entityArtifact.Name, language);
            var classElement = new ClassElement(entityArtifact.Name) { Documentation = entityArtifact.Description };
            classElement.BaseTypes.Add(new TypeReference("Entity"));
            if(entityArtifact.IsAggregateRoot)
            {
                classElement.BaseTypes.Add(new TypeReference("IAggregateRoot"));
            }
            codeFileElement.AddNamespace(domainNamespace, classElement);
            var constructor = new ConstructorElement()
            {
                Parameters = new List<ParameterElement>
                {
                    new ParameterElement("id", new TypeReference("Guid"))
                }
            };
            constructor.BaseCall = new ConstructorInitializer{ Arguments = new List<string> { "id" }};
            constructor.Body.AddCommentLine("Initialize the entity with the provided ID");
            classElement.Constructors.Add(constructor);
            classElement.Properties.Add(new PropertyElement("Id", TypeReference.Common.Guid) { Documentation = "The unique identifier of the entity" });
            foreach (var property in entityArtifact.EntityProperties)
            {
                var mappedType = language.GetMapping(property.DataType);
                var datatype = mappedType != null ? new TypeReference(mappedType.NativeTypeName) { IsNullable = property.IsNullable } : new TypeReference(property.DataType) { IsNullable = property.IsNullable };
                if (property.ValueTypeReference != null)
                {
                    datatype = new TypeReference(property.ValueTypeReference.Name) { IsNullable = property.IsNullable };
                }
                var propertyElement = new PropertyElement(property.Name, datatype)
                {
                    Documentation = (property.Description??string.Empty)+(!string.IsNullOrWhiteSpace(property.ExampleValue) ? $"\nFor example: {property.ExampleValue}" : string.Empty),
                };
                classElement.Properties.Add(propertyElement);
            }
            foreach (var relation in entityArtifact.EntityRelations)
            {
                TypeReference relationType;
                if (relation.SourceCardinality == RelationCardinality.One || relation.SourceCardinality == RelationCardinality.ZeroOrOne)
                {
                    relationType = new TypeReference(relation.TargetEntity.Name) { IsNullable = relation.SourceCardinality == RelationCardinality.ZeroOrOne };
                }
                else
                {
                    relationType = TypeReference.Generic("ICollection", new TypeReference(relation.TargetEntity.Name));
                    relationType.IsNullable = relation.SourceCardinality == RelationCardinality.ZeroOrMany;
                }
                classElement.Properties.Add(new PropertyElement(relation.SourcePropertyName, relationType) { Documentation = $"{relation.Name}: Relation to {relation.TargetEntity.Name} with cardinality {relation.TargetCardinality}" });
                classElement.Properties.Add(new PropertyElement($"{relation.SourcePropertyName}Id", TypeReference.Common.Guid) { Documentation = $"Id of the {relation.TargetEntity.Name}" });
            }
            foreach(var relation in entityArtifact.ReverseRelations)
            {
                TypeReference relationType;
                if (relation.TargetCardinality == RelationCardinality.One || relation.TargetCardinality == RelationCardinality.ZeroOrOne)
                {
                    relationType = new TypeReference(relation.SourceEntity.Name) { IsNullable = relation.TargetCardinality == RelationCardinality.ZeroOrOne };
                }
                else
                {
                    relationType = TypeReference.Generic("ICollection", new TypeReference(relation.SourceEntity.Name));
                    relationType.IsNullable = relation.TargetCardinality == RelationCardinality.ZeroOrMany;
                }
                classElement.Properties.Add(new PropertyElement(relation.TargetPropertyName, relationType) { Documentation = $"Relation to {relation.TargetEntity.Name} with cardinality {relation.TargetCardinality}" });
            }

            var codeFileArtifact = new CodeFileArtifact(codeFileElement);
            AddChildArtifactToParent(domainFolderArtifact, codeFileArtifact, e.Result);
            return Task.CompletedTask;
        }

        private async Task GenerateEntityByTemplateId(DotNetProjectArtifactCreatedEventArgs e, ScopeArtifact scope, DomainArtifact domain, EntityArtifact entityArtifact, FolderArtifact domainFolderArtifact, string domainNamespace)
        {
            try
            {
                var templateDecorator = entityArtifact.GetDecoratorOfType<EntityTemplateArtifactDecorator>();
                if (templateDecorator == null) return; // This entity is not marked for template generation
                if (string.IsNullOrEmpty(templateDecorator.TemplateId)) return;
                var template = _templateManager.GetTemplateById(templateDecorator.TemplateId);
                if (template == null) {
                    var defaultTemplateId = ENTITY_TEMPLATE_ID;
                    if (templateDecorator.TemplateId == defaultTemplateId)
                    {
                        // if it's the default template, try to create it manually
                        var templateFolderPath = _templateManager.ResolveTemplateIdToFolderPath(defaultTemplateId);
                        var templatePath = Path.Combine(templateFolderPath, $"{ENTITY_TEMPLATE_NAME}.scriban");
                        template = new ScribanFileTemplate(defaultTemplateId, templatePath)
                        {
                            // Ensure the template file is created if it doesn't exist, to give users a starting point for customizing the template
                            CreateTemplateFileIfMissing = true
                        };
                        var definitionFilePath = TemplateDefinition.GetDefinitionFilePath(templatePath);
                        if (!File.Exists(definitionFilePath))
                        {
                            var templateDefinition = new TemplateDefinition();
                            templateDefinition.TemplateName = ENTITY_TEMPLATE_NAME;
                            templateDefinition.DisplayName = "Entity Template";
                            templateDefinition.Parameters.Add(new TemplateParameter { Name = "Entity", FullyQualifiedAssemblyTypeName = typeof(EntityArtifact).AssemblyQualifiedName, Description = "The entity artifact for which the code is being generated" });
                            templateDefinition.Parameters.Add(new TemplateParameter { Name = "Domain", FullyQualifiedAssemblyTypeName = typeof(DomainArtifact).AssemblyQualifiedName, Description = "The domain artifact to which the entity belongs" });
                            templateDefinition.Parameters.Add(new TemplateParameter { Name = "Scope", FullyQualifiedAssemblyTypeName = typeof(ScopeArtifact).AssemblyQualifiedName, Description = "The scope artifact to which the entity belongs" });
                            templateDefinition.Parameters.Add(new TemplateParameter { Name = "Namespace", FullyQualifiedAssemblyTypeName = typeof(string).AssemblyQualifiedName, Description = "The namespace for the generated entity" });
                            templateDefinition.SaveToFile(definitionFilePath);
                        }
                    } else
                    {
                        _logger.LogError("Template with id {TemplateId} not found for entity {EntityName} in domain {DomainName} and scope {ScopeName}", templateDecorator.TemplateId, entityArtifact.Name, domain.Name, scope.Name);
                        return; // Template specified in the decorator not found, skip generation for this entity
                    }
                    
                }
               
                var templateEngine = _templateEngineManager.GetTemplateEnginesForTemplate(template).FirstOrDefault();
                if (templateEngine == null) return; // No template engine found for this template, skip generation for this entity
                var templateInstance = templateEngine.CreateTemplateInstance(template);
                if (templateInstance is ScribanTemplateInstance scribanTemplateInstance)
                {
                    scribanTemplateInstance.OutputFileName = $"{entityArtifact.Name}.cs";
                    
                }
                templateInstance.SetParameter("Entity", entityArtifact);
                templateInstance.SetParameter("Domain", domain);
                templateInstance.SetParameter("Scope", scope);
                templateInstance.SetParameter("Namespace", domainNamespace);
                var result = await templateEngine.RenderAsync(templateInstance, CancellationToken.None);
                if(result.Succeeded)
                {
                    foreach (var artifact in result.Artifacts)
                    {
                        AddChildArtifactToParent(domainFolderArtifact, artifact, e.Result);
                    }
                }
                else
                {
                    e.Result.Errors.AddRange(result.Errors);
                    _logger.LogError("Failed to generate entity {EntityName} for domain {DomainName} in scope {ScopeName}. Errors: {Errors}", entityArtifact.Name, domain.Name, scope.Name, string.Join(", ", result.Errors));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating entity {EntityName} for domain {DomainName} in scope {ScopeName}", entityArtifact.Name, domain.Name, scope.Name);
            }
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if(_dotnetprojectcreated_unsubscribe_handler!=null)
            {
                messageBus.Unsubscribe<DotNetProjectArtifactCreatedEventArgs>(_dotnetprojectcreated_unsubscribe_handler);
                _dotnetprojectcreated_unsubscribe_handler = null;
            }
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var name = "Entity Template";
            var description = "Generates an entity class for the domain layer.";
            
            var generatorDescription = new GeneratorSettingsDescription(GENERATOR_ID, name, description);

            var templateRequirement = new TemplateRequirement(ENTITY_TEMPLATE_ID, "Generate entity class", "{EntityName}.cs", TemplateType.Scriban);
            generatorDescription.Templates.Add(templateRequirement);

            return generatorDescription;
        }
    }
}
