namespace CodeGenerator.Domain.Languages.Constructs;

/// <summary>
/// Base class for language constructs (classes, interfaces, etc.)
/// </summary>
public abstract class LanguageConstruct
{
    /// <summary>
    /// Name of the construct
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Namespace or module containing this construct
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// Documentation comments
    /// </summary>
    public string? Documentation { get; set; }

    /// <summary>
    /// Access modifier
    /// </summary>
    public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

    /// <summary>
    /// Attributes/decorators applied to this construct
    /// </summary>
    public List<AttributeDefinition> Attributes { get; } = new();

    /// <summary>
    /// Full qualified name including namespace
    /// </summary>
    public string FullName => string.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}.{Name}";
}
