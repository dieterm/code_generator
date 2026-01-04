namespace CodeGenerator.Domain.Languages.Constructs;

/// <summary>
/// Represents a method construct
/// </summary>
public class MethodConstruct
{
    public string Name { get; set; } = string.Empty;
    public string ReturnType { get; set; } = "void";
    public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
    public bool IsStatic { get; set; }
    public bool IsVirtual { get; set; }
    public bool IsOverride { get; set; }
    public bool IsAbstract { get; set; }
    public bool IsAsync { get; set; }
    public bool IsExtensionMethod { get; set; }
    public List<ParameterConstruct> Parameters { get; } = new();
    public List<GenericTypeParameter> TypeParameters { get; } = new();
    public string? Body { get; set; }
    public string? Documentation { get; set; }
    public List<AttributeDefinition> Attributes { get; } = new();
}

/// <summary>
/// Represents a method parameter
/// </summary>
public class ParameterConstruct
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? DefaultValue { get; set; }
    public ParameterModifier Modifier { get; set; } = ParameterModifier.None;
    public List<AttributeDefinition> Attributes { get; } = new();
}

/// <summary>
/// Parameter modifiers
/// </summary>
public enum ParameterModifier
{
    None,
    Ref,
    Out,
    In,
    Params,
    This  // for extension methods
}
