using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Generators.Settings
{
    /// <summary>
    /// Defines a parameter for a generator
    /// </summary>
    public class ParameterDefinition
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Parameter type (e.g., "String", "Int32", "Boolean")<br />
        /// See supported types in ParameterDefinitionTypes-class
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = ParameterDefinitionTypes.String;

        /// <summary>
        /// Default value as object
        /// </summary>
        [JsonPropertyName("defaultValue")]
        public object? DefaultValue { get; set; }

        /// <summary>
        /// Description for UI tooltip
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Whether this parameter is required
        /// </summary>
        [JsonPropertyName("required")]
        public bool Required { get; set; } = false;

        /// <summary>
        /// Possible values for enum-like parameters
        /// </summary>
        [JsonPropertyName("possibleValues")]
        public List<object>? PossibleValues { get; set; }
    }
}
