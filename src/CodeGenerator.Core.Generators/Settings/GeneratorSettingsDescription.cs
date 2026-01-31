using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Core.Settings;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Templates.Settings;

namespace CodeGenerator.Core.Generators.Settings
{
    /// <summary>
    /// Describes a generator's settings and requirements.
    /// This is the definition/schema of what settings a generator needs.
    /// Implements IGeneratorSettingsProvider for integration with GeneratorSettingsManager.
    /// </summary>
    public class GeneratorSettingsDescription : IGeneratorSettingsProvider
    {
        /// <summary>
        /// Unique identifier for this generator
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of what this generator produces
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Category for grouping in UI (e.g., "Domain", "Infrastructure", "Presentation")
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Icon key for UI display
        /// </summary>
        public string? IconKey { get; }

        /// <summary>
        /// List of template requirements for this generator
        /// </summary>
        public List<TemplateRequirement> Templates { get; } = new List<TemplateRequirement>();

        /// <summary>
        /// Generator-specific parameters (optional ApplicationSettingsBase)
        /// </summary>
        public Dictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>();

        /// <summary>
        /// Parameter definitions for UI generation
        /// </summary>
        public List<ParameterDefinition> ParameterDefinitions { get; } = new List<ParameterDefinition>();
        public List<string> DependingGeneratorIds { get; } = new List<string>();
        public GeneratorSettingsDescription(
            string id, 
            string name, 
            string description, 
            string category = "General",
            string? iconKey = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Category = category;
            IconKey = iconKey;
        }

        #region IGeneratorSettingsProvider Implementation

        string IGeneratorSettingsProvider.GeneratorId => Id;
        string IGeneratorSettingsProvider.GeneratorName => Name;
        string IGeneratorSettingsProvider.GeneratorDescription => Description;
        string IGeneratorSettingsProvider.Category => Category;

        /// <summary>
        /// Create a GeneratorSettings instance with default values from this description
        /// </summary>
        public Core.Settings.Generators.GeneratorSettings GetDefaultSettings()
        {
            var settings = new Core.Settings.Generators.GeneratorSettings(Id)
            {
                Enabled = true,
                Name = Name,
                Description = Description,
                Category = Category
            };

            // Copy templates
            foreach (var template in Templates)
            {
                settings.Templates.Add(template.ToSettings());
            }

            // Copy parameter defaults
            if (Parameters != null)
            {
                foreach (var parDef in ParameterDefinitions)
                {
                    settings.Parameters[parDef.Name] = parDef.DefaultValue;
                }
            }

            return settings;
        }

        #endregion
    }
}
