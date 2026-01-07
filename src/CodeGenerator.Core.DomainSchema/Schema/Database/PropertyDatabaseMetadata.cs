using System.Text.Json.Serialization;

namespace CodeGenerator.Core.DomainSchema.Schema;

/// <summary>
/// Property-level database metadata
/// </summary>
public class PropertyDatabaseMetadata
{
    [JsonPropertyName("columnName")]
    public string? ColumnName { get; set; }

    [JsonPropertyName("columnType")]
    public string? ColumnType { get; set; }

    [JsonPropertyName("isPrimaryKey")]
    public bool IsPrimaryKey { get; set; }

    [JsonPropertyName("isIdentity")]
    public bool IsIdentity { get; set; }

    [JsonPropertyName("isForeignKey")]
    public bool IsForeignKey { get; set; }

    [JsonPropertyName("foreignKeyReference")]
    public ForeignKeyReference? ForeignKeyReference { get; set; }

    [JsonPropertyName("isUnique")]
    public bool IsUnique { get; set; }

    [JsonPropertyName("isIndexed")]
    public bool IsIndexed { get; set; }

    [JsonPropertyName("defaultConstraint")]
    public string? DefaultConstraint { get; set; }

    [JsonPropertyName("collation")]
    public string? Collation { get; set; }

    [JsonPropertyName("precision")]
    public int? Precision { get; set; }

    [JsonPropertyName("scale")]
    public int? Scale { get; set; }

    [JsonPropertyName("computedColumnSql")]
    public string? ComputedColumnSql { get; set; }

    [JsonPropertyName("isStored")]
    public bool IsStored { get; set; }
}
