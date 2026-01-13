using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Workspaces.Models
{
    public class ColumnDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("dataType")]
        public string DataType { get; set; } = string.Empty;
        [JsonPropertyName("isNullable")]
        public bool IsNullable { get; set; }
        [JsonPropertyName("isPrimaryKey")]
        public bool IsPrimaryKey { get; set; }
        [JsonPropertyName("isAutoIncrement")]
        public bool IsAutoIncrement { get; set; }
        [JsonPropertyName("maxLength")]
        public int? MaxLength { get; set; }
        [JsonPropertyName("precision")]
        public int? Precision { get; set; }
        [JsonPropertyName("scale")]
        public int? Scale { get; set; }
        [JsonPropertyName("defaultValue")]
        public string? DefaultValue { get; set; }
        [JsonPropertyName("foreignKeyTable")]
        public string? ForeignKeyTable { get; set; }
        [JsonPropertyName("foreignKeyColumn")]
        public string? ForeignKeyColumn { get; set; }
        /// <summary>
        /// Additional metadata (e.g., decorators, custom attributes)
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

    }
}