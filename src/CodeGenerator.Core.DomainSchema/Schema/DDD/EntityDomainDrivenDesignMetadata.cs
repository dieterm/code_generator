using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Models.Schema;

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
}
