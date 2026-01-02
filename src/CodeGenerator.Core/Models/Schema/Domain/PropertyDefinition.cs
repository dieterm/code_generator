using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Models.Schema;

/// <summary>
/// Property definition in the schema
/// </summary>
public class PropertyDefinition
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("default")]
    public object? Default { get; set; }

    [JsonPropertyName("enum")]
    public List<object>? Enum { get; set; }

    [JsonPropertyName("const")]
    public object? Const { get; set; }

    [JsonPropertyName("minimum")]
    public decimal? Minimum { get; set; }

    [JsonPropertyName("maximum")]
    public decimal? Maximum { get; set; }

    [JsonPropertyName("exclusiveMinimum")]
    public decimal? ExclusiveMinimum { get; set; }

    [JsonPropertyName("exclusiveMaximum")]
    public decimal? ExclusiveMaximum { get; set; }

    [JsonPropertyName("minLength")]
    public int? MinLength { get; set; }

    [JsonPropertyName("maxLength")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("pattern")]
    public string? Pattern { get; set; }

    [JsonPropertyName("items")]
    public PropertyDefinition? Items { get; set; }

    [JsonPropertyName("minItems")]
    public int? MinItems { get; set; }

    [JsonPropertyName("maxItems")]
    public int? MaxItems { get; set; }

    [JsonPropertyName("uniqueItems")]
    public bool? UniqueItems { get; set; }

    [JsonPropertyName("$ref")]
    public string? Ref { get; set; }

    [JsonPropertyName("x-codegen")]
    public PropertyCodeGenMetadata? CodeGenMetadata { get; set; }

    [JsonPropertyName("x-db")]
    public PropertyDatabaseMetadata? DatabaseMetadata { get; set; }

    [JsonPropertyName("x-ddd")]
    public PropertyDomainDrivenDesignMetadata? DomainDrivenDesignMetadata { get; set; }
}
