using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Workspaces.Models
{
    /// <summary>
    /// Definition for a database view
    /// </summary>
    public class ViewDefinition
    {
        /// <summary>
        /// View name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Schema name (e.g., "dbo")
        /// </summary>
        [JsonPropertyName("schema")]
        public string Schema { get; set; } = "dbo";

        /// <summary>
        /// SQL definition of the view
        /// </summary>
        [JsonPropertyName("definition")]
        public string? Definition { get; set; }

        /// <summary>
        /// List of columns in the view
        /// </summary>
        [JsonPropertyName("columns")]
        public List<ColumnDefinition> Columns { get; set; } = new();

        /// <summary>
        /// Additional metadata (e.g., decorators, custom attributes)
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

    }
}
