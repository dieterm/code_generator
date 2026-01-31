using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates.Settings
{
    public class TemplateEngineSettings
    {
        /// <summary>
        /// Parameterless constructor for deserialization
        /// </summary>
        public TemplateEngineSettings()
        {
            TemplateEngineId = string.Empty;
            Templates = new List<TemplateRequirementSettings>();
            Parameters = new Dictionary<string, object?>();
        }

        public TemplateEngineSettings(string templateEngineId)
        {
            TemplateEngineId = templateEngineId;
            Templates = new List<TemplateRequirementSettings>();
            Parameters = new Dictionary<string, object?>();
        }

        /// <summary>
        /// Template engine identifier
        /// </summary>
        [JsonPropertyName("templateEngineId")]
        public string TemplateEngineId { get; set; }
        /// <summary>
        /// Display name of the generator
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Description of the generator
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Category for grouping (e.g., "Domain", "Infrastructure")
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; set; }
        /// <summary>
        /// Whether this template engine is enabled
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;


        /// <summary>
        /// List of template configurations for this generator
        /// </summary>
        [JsonPropertyName("templates")]
        public List<TemplateRequirementSettings> Templates { get; set; }

        /// <summary>
        /// Generator-specific parameters stored as key-value pairs
        /// </summary>
        [JsonPropertyName("parameters")]
        public Dictionary<string, object?> Parameters { get; set; }

        /// <summary>
        /// Get a parameter value by key
        /// </summary>
        public T? GetParameter<T>(string key, T? defaultValue = default)
        {
            if (Parameters.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }

                // Try to convert from JsonElement if loaded from JSON
                if (value is System.Text.Json.JsonElement jsonElement)
                {
                    try
                    {
                        return System.Text.Json.JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Set a parameter value
        /// </summary>
        public void SetParameter<T>(string key, T? value)
        {
            Parameters[key] = value;
        }

        /// <summary>
        /// Get a template by its ID
        /// </summary>
        public TemplateRequirementSettings? GetTemplate(string templateId)
        {
            return Templates.FirstOrDefault(t => t.TemplateId == templateId);
        }

        /// <summary>
        /// Update or add a template
        /// </summary>
        public void SetTemplate(TemplateRequirementSettings template)
        {
            var existing = Templates.FirstOrDefault(t => t.TemplateId == template.TemplateId);
            if (existing != null)
            {
                Templates.Remove(existing);
            }
            Templates.Add(template);
        }
    }
}
