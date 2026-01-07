using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeGenerator.Core.DomainSchema.Schema;

/// <summary>
/// Entity-level Domain-Driven Design metadata
/// </summary>
public class EntityDomainDrivenDesignMetadata
{
    /// <summary>
    /// Indicates if this entity is a Value Object (immutable, no identity)
    /// </summary>
    [JsonPropertyName("valueObject")]
    public bool ValueObject { get; set; }

    /// <summary>
    /// Indicates if this entity is an Aggregate Root (entry point for the aggregate)
    /// </summary>
    [JsonPropertyName("aggregateRoot")]
    public bool AggregateRoot { get; set; }

    /// <summary>
    /// Indicates if this entity has a hierarchical structure (parent-child relationship)
    /// </summary>
    [JsonPropertyName("hierarchical")]
    public bool Hierarchical { get; set; }

    /// <summary>
    /// Additional/unknown properties that are not explicitly defined
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
