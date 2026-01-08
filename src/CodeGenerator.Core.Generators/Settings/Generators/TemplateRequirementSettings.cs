using CodeGenerator.Core.Templates;
using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Settings.Generators
{
    /// <summary>
    /// Represents a template requirement for a generator.
    /// JSON serializable for settings persistence.
    /// </summary>
    public class TemplateRequirementSettings
    {
        /// <summary>
        /// Parameterless constructor for JSON deserialization
        /// </summary>
        public TemplateRequirementSettings()
        {
            TemplateId = string.Empty;
            Description = string.Empty;
            OutputFileNamePattern = string.Empty;
            TemplateType = TemplateType.Scriban;
        }

        public TemplateRequirementSettings(string templateId, string description, string outputFileNamePattern, TemplateType templateType)
        {
            TemplateId = templateId;
            Description = description;
            OutputFileNamePattern = outputFileNamePattern;
            TemplateType = templateType;
        }

        /// <summary>
        /// Id of the template as to be defined in settings
        /// </summary>
        [JsonPropertyName("templateId")]
        public string TemplateId { get; set; }

        /// <summary>
        /// Description of what the template does
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Type of template (eg. Scriban, T4, TextFile,...)
        /// </summary>
        [JsonPropertyName("templateType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TemplateType TemplateType { get; set; }

        /// <summary>
        /// Output file name pattern (e.g. '{{EntityName}}Controller.cs')
        /// </summary>
        [JsonPropertyName("outputFileNamePattern")]
        public string OutputFileNamePattern { get; set; }

        /// <summary>
        /// Path to the template file (set by user in settings)
        /// </summary>
        [JsonPropertyName("templateFilePath")]
        public string? TemplateFilePath { get; set; }

        /// <summary>
        /// Whether this template is enabled
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;
    }
}
