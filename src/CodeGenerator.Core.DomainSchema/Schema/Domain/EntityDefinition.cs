using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Models.Schema;

/// <summary>
/// Entity definition in the schema
/// </summary>
public class EntityDefinition
{
    [JsonIgnore]
    public string Key { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, PropertyDefinition>? Properties { get; set; }

    [JsonPropertyName("required")]
    public List<string>? Required { get; set; }

    [JsonPropertyName("x-codegen")]
    public EntityCodeGenMetadata? CodeGenMetadata { get; set; }

    [JsonPropertyName("x-db")]
    public EntityDatabaseMetadata? DatabaseMetadata { get; set; }

    [JsonPropertyName("x-ddd")]
    public EntityDomainDrivenDesignMetadata? DomainDrivenDesignMetadata { get; set; }

    [JsonPropertyName("allOf")]
    public List<SchemaReference>? AllOf { get; set; }

    [JsonPropertyName("oneOf")]
    public List<SchemaReference>? OneOf { get; set; }

    [JsonPropertyName("anyOf")]
    public List<SchemaReference>? AnyOf { get; set; }
}
