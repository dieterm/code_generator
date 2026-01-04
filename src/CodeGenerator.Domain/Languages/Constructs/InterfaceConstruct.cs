namespace CodeGenerator.Domain.Languages.Constructs;

/// <summary>
/// Represents an interface construct
/// </summary>
public class InterfaceConstruct : LanguageConstruct
{
    /// <summary>
    /// Base interfaces
    /// </summary>
    public List<string> BaseInterfaces { get; } = new();

    /// <summary>
    /// Generic type parameters
    /// </summary>
    public List<GenericTypeParameter> TypeParameters { get; } = new();

    /// <summary>
    /// Property signatures
    /// </summary>
    public List<PropertyConstruct> Properties { get; } = new();

    /// <summary>
    /// Method signatures
    /// </summary>
    public List<MethodConstruct> Methods { get; } = new();

    /// <summary>
    /// Event signatures
    /// </summary>
    public List<EventConstruct> Events { get; } = new();
}
