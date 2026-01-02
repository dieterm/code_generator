using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Models.Domain;
/// <summary>
/// Navigation property for relationships
/// </summary>
public class NavigationProperty
{
    /// <summary>
    /// Property name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Target entity name
    /// </summary>
    public string TargetEntity { get; set; } = string.Empty;

    /// <summary>
    /// Relationship type
    /// </summary>
    public RelationshipType RelationshipType { get; set; }

    /// <summary>
    /// Foreign key property name in this entity
    /// </summary>
    public string? ForeignKeyProperty { get; set; }

    /// <summary>
    /// Inverse navigation property name
    /// </summary>
    public string? InverseProperty { get; set; }

    /// <summary>
    /// Whether this is the principal side
    /// </summary>
    public bool IsPrincipal { get; set; }

    /// <summary>
    /// Whether this property is nullable
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// For many-to-many, the join entity name
    /// </summary>
    public string? JoinEntity { get; set; }
}
