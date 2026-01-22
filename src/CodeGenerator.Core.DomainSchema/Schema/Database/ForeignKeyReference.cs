using System.Text.Json.Serialization;

namespace CodeGenerator.Core.DomainSchema.Schema;

/// <summary>
/// Foreign key reference definition
/// </summary>
public class ForeignKeyReference
{
    [JsonPropertyName("table")]
    public string Table { get; set; } = string.Empty;

    [JsonPropertyName("column")]
    public string Column { get; set; } = string.Empty;

    [JsonPropertyName("onDelete")]
    public string OnDelete { get; set; } = "NoAction";

    [JsonPropertyName("onUpdate")]
    public string OnUpdate { get; set; } = "NoAction";

    [JsonPropertyName("constraintName")]
    public string? ConstraintName { get; set; }
}
