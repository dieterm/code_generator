using CodeGenerator.Core.Templates;
using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Generators.Settings
{
    /// <summary>
    /// Represents a template requirement for a generator.
    /// This is the runtime version used by generators.
    /// </summary>
    public class TemplateRequirement
    {
        /// <summary>
        /// Parameterless constructor for JSON deserialization
        /// </summary>
        public TemplateRequirement()
        {
            TemplateId = string.Empty;
            Description = string.Empty;
            OutputFileNamePattern = string.Empty;
            TemplateType = TemplateType.Scriban;
        }

        public TemplateRequirement(string templateId, string description, string outputFileNamePattern, TemplateType templateType)
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
        
        ///// <summary>
        ///// Any optional subfolder path after the "@workspace/<generatorname>/" part of the TemplateId
        ///// </summary>
        //[JsonPropertyName("templatePath")]
        //public string? TemplatePath { get; set; }
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

        /// <summary>
        /// Convert to settings version for persistence
        /// </summary>
        public Core.Settings.Generators.TemplateRequirementSettings ToSettings()
        {
            return new Core.Settings.Generators.TemplateRequirementSettings
            {
                TemplateId = TemplateId,
                Description = Description,
                OutputFileNamePattern = OutputFileNamePattern,
                TemplateType = TemplateType,
                TemplateFilePath = TemplateFilePath,
                Enabled = Enabled
            };
        }

        /// <summary>
        /// Create from settings version
        /// </summary>
        public static TemplateRequirement FromSettings(Core.Settings.Generators.TemplateRequirementSettings settings)
        {
            return new TemplateRequirement
            {
                TemplateId = settings.TemplateId,
                Description = settings.Description,
                OutputFileNamePattern = settings.OutputFileNamePattern,
                TemplateType = (TemplateType)(int)settings.TemplateType,
                TemplateFilePath = settings.TemplateFilePath,
                Enabled = settings.Enabled
            };
        }
    }
}
