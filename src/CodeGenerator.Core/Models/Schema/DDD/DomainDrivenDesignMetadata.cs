using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Models.Schema;

/// <summary>
/// Root-level Domain-Driven Design metadata
/// </summary>
public class DomainDrivenDesignMetadata
{
    /// <summary>
    /// The bounded context this domain belongs to
    /// </summary>
    [JsonPropertyName("boundedContext")]
    public string? BoundedContext { get; set; }
}
