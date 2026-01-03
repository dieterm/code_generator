using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Models.Domain;

/// <summary>
/// Complete domain context for code generation
/// </summary>
public class DomainContext
{
    /// <summary>
    /// Context name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Root namespace
    /// </summary>
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// All entities in this context
    /// </summary>
    public List<EntityModel> Entities { get; set; } = new();

    /// <summary>
    /// All enums defined
    /// </summary>
    public List<EnumModel> Enums { get; set; } = new();

    /// <summary>
    /// Code generation metadata
    /// </summary>
    public CodeGenMetadata? CodeGenMetadata { get; set; }

    /// <summary>
    /// Database metadata
    /// </summary>
    public DatabaseMetadata? DatabaseMetadata { get; set; }

    /// <summary>
    /// Domain Driven Design metadata
    /// </summary>
    public DomainDrivenDesignMetadata? DomainDrivenDesignMetadata { get; set; }
    /// <summary>
    /// Original schema
    /// </summary>
    public DomainSchema? OriginalSchema { get; set; }
}
