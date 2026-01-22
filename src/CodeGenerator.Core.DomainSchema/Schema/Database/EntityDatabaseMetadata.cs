using System.Text.Json.Serialization;

namespace CodeGenerator.Core.DomainSchema.Schema;

/// <summary>
/// Entity-level database metadata
/// </summary>
public class EntityDatabaseMetadata
{
    [JsonPropertyName("tableName")]
    public string? TableName { get; set; }

    [JsonPropertyName("schema")]
    public string? Schema { get; set; }

    [JsonPropertyName("isView")]
    public bool IsView { get; set; }

    [JsonPropertyName("viewDefinition")]
    public string? ViewDefinition { get; set; }

    [JsonPropertyName("indexes")]
    public List<IndexDefinition>? Indexes { get; set; }

    [JsonPropertyName("uniqueConstraints")]
    public List<UniqueConstraint>? UniqueConstraints { get; set; }

    [JsonPropertyName("checkConstraints")]
    public List<CheckConstraint>? CheckConstraints { get; set; }

    [JsonPropertyName("triggers")]
    public List<TriggerDefinition>? Triggers { get; set; }
}
