namespace CodeGenerator.Domain.Languages.Constructs;

/// <summary>
/// Represents a class construct
/// </summary>
public class ClassConstruct : LanguageConstruct
{
    /// <summary>
    /// Base class
    /// </summary>
    public string? BaseClass { get; set; }

    /// <summary>
    /// Implemented interfaces
    /// </summary>
    public List<string> Interfaces { get; } = new();

    /// <summary>
    /// Whether the class is abstract
    /// </summary>
    public bool IsAbstract { get; set; }

    /// <summary>
    /// Whether the class is sealed/final
    /// </summary>
    public bool IsSealed { get; set; }

    /// <summary>
    /// Whether the class is static
    /// </summary>
    public bool IsStatic { get; set; }

    /// <summary>
    /// Whether the class is partial
    /// </summary>
    public bool IsPartial { get; set; }

    /// <summary>
    /// Whether this is a record type
    /// </summary>
    public bool IsRecord { get; set; }

    /// <summary>
    /// Generic type parameters
    /// </summary>
    public List<GenericTypeParameter> TypeParameters { get; } = new();

    /// <summary>
    /// Properties of the class
    /// </summary>
    public List<PropertyConstruct> Properties { get; } = new();

    /// <summary>
    /// Methods of the class
    /// </summary>
    public List<MethodConstruct> Methods { get; } = new();

    /// <summary>
    /// Fields of the class
    /// </summary>
    public List<FieldConstruct> Fields { get; } = new();

    /// <summary>
    /// Constructors of the class
    /// </summary>
    public List<ConstructorConstruct> Constructors { get; } = new();

    /// <summary>
    /// Nested types
    /// </summary>
    public List<LanguageConstruct> NestedTypes { get; } = new();
}
