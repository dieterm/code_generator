using CodeGenerator.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates.Settings
{
    public class TemplateEngineSettingsDescription
    {
        public TemplateEngineSettings GetDefaultSettings()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unique identifier for this template engine
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of what this template engine produces
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
        /// List of template requirements for this template engine
        /// </summary>
        public List<TemplateRequirement> Templates { get; } = new List<TemplateRequirement>();

        /// <summary>
        /// template engine-specific parameters (optional ApplicationSettingsBase)
        /// </summary>
        public Dictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>();

        /// <summary>
        /// Parameter definitions for UI generation
        /// </summary>
        public List<ParameterDefinition> ParameterDefinitions { get; } = new List<ParameterDefinition>();
        public List<string> DependingTemplateIds { get; } = new List<string>();
        public TemplateEngineSettingsDescription(
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
    }
}
