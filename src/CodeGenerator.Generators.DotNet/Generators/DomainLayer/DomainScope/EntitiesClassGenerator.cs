using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Generators;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Shared;
using CodeGenerator.TemplateEngines.Scriban;
using CodeGenerator.UserControls.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators.DomainLayer.DomainScope
{
    public class EntitiesClassGenerator : GeneratorBase
    {
        private Func<CreatedArtifactEventArgs, Task>? _unsubscribe_handler;
        private readonly ILogger<EntitiesClassGenerator> _logger;

        public EntitiesClassGenerator(ILogger<EntitiesClassGenerator> logger)
        {
            _logger = logger;
        }

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            // Use async Subscribe variant for proper async handling
            _unsubscribe_handler = messageBus.Subscribe<CreatedArtifactEventArgs>(OnDotNetDomainProjectCreated, FilterDotNetDomainProject);
        }

        private bool FilterDotNetDomainProject(CreatedArtifactEventArgs e)
        {
            var domainProjectArtifact = e.Artifact as DotNetProjectArtifact;
            if(domainProjectArtifact== null) return false;
            var domainLayerArtifact = domainProjectArtifact.Parent as DomainLayerArtifact;
            if(domainLayerArtifact == null) return false;
            var hasDecorator = domainLayerArtifact.HasDecorator<DomainArtifactRefDecorator>();
            return hasDecorator;
        }

        private async Task OnDotNetDomainProjectCreated(CreatedArtifactEventArgs e)
        {
            var dotNetProjectArtifact = e.Artifact as DotNetProjectArtifact;
            if (dotNetProjectArtifact == null)
            {
                _logger.LogError("Artifact is not a DotNetProjectArtifact.");
                throw new InvalidOperationException("Artifact is not a DotNetProjectArtifact.");
            }

            var layerArtifact = dotNetProjectArtifact.Parent as CodeArchitectureLayerArtifact;
            if (layerArtifact == null)
            {
                _logger.LogError("Parent artifact is not a CodeArchitectureLayerArtifact.");
                throw new InvalidOperationException("Parent artifact is not a CodeArchitectureLayerArtifact.");
            }
            var domainArtifactRefDecorator = layerArtifact.GetDecoratorOfType<DomainArtifactRefDecorator>();
            if(domainArtifactRefDecorator == null)
            {
                _logger.LogError("DomainArtifactRefDecorator not found on layer artifact.");
                throw new InvalidOperationException("DomainArtifactRefDecorator not found on layer artifact.");
            }
            _logger.LogInformation("Generating entity classes for domain {DomainName} on layer {LayerName} scope {Scope}.", domainArtifactRefDecorator.DomainArtifact.Name, layerArtifact.Layer, layerArtifact.Scope);
            var domainArtifact = domainArtifactRefDecorator.DomainArtifact;

            var settings = GetSettings();
            var entitiesSettings = new EntitiesClassGeneratorSettings(settings);
            //var templateSettings = settings.GetTemplate(entitiesSettings.TemplateId?? EntitiesClassGeneratorSettings.ENTITIES_CLASS_TEMPLATE_ID);
            //templateSettings.

            var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
            var template = templateManager.GetTemplateById(entitiesSettings.TemplateId);
            if(template == null)
            {
                _logger.LogError("Template with ID {TemplateId} not found.", entitiesSettings.TemplateId);
                throw new InvalidOperationException($"Template with ID {entitiesSettings.TemplateId} not found.");
            }
           // template.UseCaching = true;
            var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
            var templateEngine =templateEngineManager.GetTemplateEnginesForTemplate(template).FirstOrDefault();
            if(templateEngine == null)
            {
                _logger.LogError("No template engine found for template ID {TemplateId}.", entitiesSettings.TemplateId);
                throw new InvalidOperationException($"No template engine found for template ID {entitiesSettings.TemplateId}.");
            }
            foreach (var entityArtifact in domainArtifact.Entities.GetEntities())
            {
                var templateInstance = templateEngine.CreateTemplateInstance(template);
                
                templateInstance.SetParameter("Entity", entityArtifact);
                templateInstance.SetParameter("Domain", domainArtifact);
                templateInstance.SetParameter("Layer", layerArtifact);
                templateInstance.SetParameter("DotNetProject", dotNetProjectArtifact);
                templateInstance.SetParameter("BaseClassName", entitiesSettings.BaseClassName);
                if (templateInstance.Template is ScribanFileTemplate scribanTemplate)
                {
                    var templateDefinition = TemplateDefinition.LoadForTemplate(scribanTemplate.FilePath);
                    if (templateDefinition == null) {
                        templateDefinition = TemplateDefinition.CreateDefault(scribanTemplate.FilePath);
                        templateDefinition.Description = $"Used by {typeof(EntitiesClassGenerator).Name} to generates a C# class for a domain entity.";
                    }
                    var entityParam = templateDefinition?.Parameters.FirstOrDefault(p => p.Name == "Entity");
                    if (entityParam == null)
                    {
                        templateDefinition?.Parameters.Add(new TemplateParameter
                        {
                            Name = "Entity",
                            FullyQualifiedAssemblyTypeName = typeof(EntityArtifact).AssemblyQualifiedName!,
                            Description = "The entity for which the class is being generated."
                        });
                    }
                    var domainParam = templateDefinition?.Parameters.FirstOrDefault(p => p.Name == "Domain");
                    if (domainParam == null)
                    {
                        templateDefinition?.Parameters.Add(new TemplateParameter
                        {
                            Name = "Domain",
                            FullyQualifiedAssemblyTypeName = typeof(DomainArtifact).AssemblyQualifiedName!,
                            Description = "The domain containing the entity."
                        });
                    }
                    var layerParam = templateDefinition?.Parameters.FirstOrDefault(p => p.Name == "Layer");
                    if (layerParam == null)
                    {
                        templateDefinition?.Parameters.Add(new TemplateParameter
                        {
                            Name = "Layer",
                            FullyQualifiedAssemblyTypeName = typeof(CodeArchitectureLayerArtifact).AssemblyQualifiedName!,
                            Description = "The code architecture layer containing the domain."
                        });
                    }
                    var dotNetProjectParam = templateDefinition?.Parameters.FirstOrDefault(p => p.Name == "DotNetProject");
                    if (dotNetProjectParam == null)
                    {
                        templateDefinition?.Parameters.Add(new TemplateParameter
                        {
                            Name = "DotNetProject",
                            FullyQualifiedAssemblyTypeName = typeof(DotNetProjectArtifact).AssemblyQualifiedName!,
                            Description = "The .NET project artifact where the generated class will be added."
                        });
                    }
                    var baseClassNameParam = templateDefinition?.Parameters.FirstOrDefault(p => p.Name == "BaseClassName");
                    if (baseClassNameParam == null)
                    {
                        templateDefinition?.Parameters.Add(new TemplateParameter
                        {
                            Name = "BaseClassName",
                            FullyQualifiedAssemblyTypeName = typeof(string).AssemblyQualifiedName!,
                            Description = "The base class name for the generated entity class."
                        });
                    }
                    templateDefinition?.SaveForTemplate(scribanTemplate.FilePath);
                }
                if(templateInstance is ScribanTemplateInstance scribanTemplateInstance)
                {
                    scribanTemplateInstance.OutputFileName = $"{entityArtifact.Name}.cs";
                }
                var output = await templateEngine.RenderAsync(templateInstance);
                if(output == null || !output.Succeeded)
                {
                    _logger.LogError("Template rendering failed for entity {EntityName}.", entityArtifact.Name);
                    throw new InvalidOperationException($"Template rendering failed for entity {entityArtifact.Name}.");
                }
                foreach (var artifact in output.Artifacts)
                {
                    AddChildArtifactToParent(dotNetProjectArtifact, artifact, e.Result);
                }
            }

        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if (_unsubscribe_handler != null)
                messageBus.Unsubscribe<CreatedArtifactEventArgs>(_unsubscribe_handler);
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            return EntitiesClassGeneratorSettings.CreateDescription();
        }
    }
}
