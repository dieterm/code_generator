using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Models.Schema;

/// <summary>
/// Root schema definition compliant with JSON Schema 2020-12
/// </summary>
public class DomainSchema
{
    [JsonPropertyName("$schema")]
    public string Schema { get; set; } = "https://json-schema.org/draft/2020-12/schema";

    [JsonPropertyName("$id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("$defs")]
    public Dictionary<string, EntityDefinition>? Definitions { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, PropertyDefinition>? Properties { get; set; }

    [JsonPropertyName("required")]
    public List<string>? Required { get; set; }

    [JsonPropertyName("x-codegen")]
    public CodeGenMetadata? CodeGenMetadata { get; set; }

    [JsonPropertyName("x-db")]
    public DatabaseMetadata? DatabaseMetadata { get; set; }

    [JsonPropertyName("x-ddd")]
    public DomainDrivenDesignMetadata? DomainDrivenDesignMetadata { get; set; }
}
