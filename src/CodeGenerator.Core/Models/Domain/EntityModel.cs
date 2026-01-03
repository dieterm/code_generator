using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Models.Domain;

/// <summary>
/// Parsed and normalized entity model ready for code generation
/// </summary>
public class EntityModel
{
    /// <summary>
    /// Entity name (from schema key)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Fully qualified class name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Namespace for this entity
    /// </summary>
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    /// Description from schema
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Base class if any
    /// </summary>
    public string? BaseClass { get; set; }

    /// <summary>
    /// Interfaces to implement
    /// </summary>
    public List<string> Interfaces { get; set; } = new();

    /// <summary>
    /// Properties of this entity
    /// </summary>
    public List<PropertyModel> Properties { get; set; } = new();

    /// <summary>
    /// Navigation properties (relationships)
    /// </summary>
    public List<NavigationProperty> NavigationProperties { get; set; } = new();

    /// <summary>
    /// Primary key properties
    /// </summary>
    public List<PropertyModel> PrimaryKeyProperties { get; set; } = new();

    /// <summary>
    /// Whether this is an abstract class
    /// </summary>
    public bool IsAbstract { get; set; }

    /// <summary>
    /// Whether this is a sealed class
    /// </summary>
    public bool IsSealed { get; set; }

    /// <summary>
    /// Whether this is an owned type (EF Core)
    /// </summary>
    public bool IsOwnedType { get; set; }

    /// <summary>
    /// Code generation settings
    /// </summary>
    public EntityCodeGenMetadata? CodeGenSettings { get; set; }

    /// <summary>
    /// Database mapping settings
    /// </summary>
    public EntityDatabaseMetadata? DatabaseSettings { get; set; }

    /// <summary>
    /// Domain Drive Design settings
    /// </summary>
    public EntityDomainDrivenDesignMetadata? DomainDrivenDesignMetadata { get; set; }

    /// <summary>
    /// Custom attributes to apply
    /// </summary>
    public List<AttributeModel> Attributes { get; set; } = new();

    /// <summary>
    /// Original schema definition
    /// </summary>
    public EntityDefinition? OriginalDefinition { get; set; }
}
