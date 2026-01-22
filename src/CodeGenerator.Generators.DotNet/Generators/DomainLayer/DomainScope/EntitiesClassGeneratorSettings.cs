using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGenerator.Generators.DotNet.Generators.DomainLayer.DomainScope
{
    public class EntitiesClassGeneratorSettings
    {
        public const string ENTITIES_CLASS_TEMPLATE_ID = "EntitiesClassTemplate";
        private readonly GeneratorSettings _settings;

        public EntitiesClassGeneratorSettings(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public string? BaseClassName
        {
            get => _settings.GetParameter<string>(nameof(BaseClassName));
            set => _settings.SetParameter<string>(nameof(BaseClassName), value);
        }

        public string? TemplateId
        {
            get => _settings.GetParameter<string>(nameof(TemplateId));
            set => _settings.SetParameter<string>(nameof(TemplateId), value);
        }

        public static GeneratorSettingsDescription CreateDescription()
        {
            var settingsDescription = new GeneratorSettingsDescription("DotNet.DomainLayer.DomainScope.EntitiesClassGenerator", "Entities Class Generator", "Generates entity classes for the domain layer.", "Domain Layer");

            // ProjectType
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = nameof(BaseClassName),
                Description = "Base class where all entity classes will inherit from. Leave blank for no base class.",
                Type = ParameterDefinitionTypes.String,
                DefaultValue = string.Empty,
                Required = false
            });
            settingsDescription.ParameterDefinitions.Add(new ParameterDefinition
            {
                Name = nameof(TemplateId),
                Description = "Template ID for the entities class generator.",
                Type = ParameterDefinitionTypes.Template,
                PossibleValues = new List<object> { TemplateType.Scriban },
                DefaultValue = ENTITIES_CLASS_TEMPLATE_ID,
                Required = true
            });
            return settingsDescription;
            
        }
    }
}
