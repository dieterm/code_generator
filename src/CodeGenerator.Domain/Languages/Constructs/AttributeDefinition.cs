namespace CodeGenerator.Domain.Languages.Constructs;

/// <summary>
/// Represents an attribute/annotation on a construct
/// </summary>
public class AttributeDefinition
{
    /// <summary>
    /// Attribute name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Positional arguments
    /// </summary>
    public List<object?> Arguments { get; } = new();

    /// <summary>
    /// Named arguments
    /// </summary>
    public Dictionary<string, object?> NamedArguments { get; } = new();
}
