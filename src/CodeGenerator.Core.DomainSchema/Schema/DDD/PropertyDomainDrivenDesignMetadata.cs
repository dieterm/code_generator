using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Models.Schema;

/// <summary>
/// Property-level Domain-Driven Design metadata
/// </summary>
public class PropertyDomainDrivenDesignMetadata
{
    /// <summary>
    /// Reference to another entity (for relationships)
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    /// <summary>
    /// Indicates if the reference is optional (nullable relationship)
    /// </summary>
    [JsonPropertyName("optional")]
    public bool Optional { get; set; }

    /// <summary>
    /// Indicates if this property is a self-reference (hierarchical structure)
    /// </summary>
    [JsonPropertyName("selfReference")]
    public bool SelfReference { get; set; }
}
