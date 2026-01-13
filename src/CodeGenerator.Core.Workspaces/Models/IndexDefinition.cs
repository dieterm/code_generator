using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Workspaces.Models
{
    public class IndexDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("isUnique")]
        public bool IsUnique { get; set; }
        [JsonPropertyName("isClustered")]
        public bool IsClustered { get; set; }
        [JsonPropertyName("columnNames")]
        public List<string> ColumnNames { get; set; } = new();
        /// <summary>
        /// Additional metadata (e.g., decorators, custom attributes)
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }
    }
}