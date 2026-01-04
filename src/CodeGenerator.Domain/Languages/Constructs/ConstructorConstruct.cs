namespace CodeGenerator.Domain.Languages.Constructs;

/// <summary>
/// Represents a constructor construct
/// </summary>
public class ConstructorConstruct
{
    public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
    public bool IsStatic { get; set; }
    public List<ParameterConstruct> Parameters { get; } = new();
    public string? BaseCall { get; set; }  // : base(...) or : this(...)
    public string? Body { get; set; }
    public string? Documentation { get; set; }
    public List<AttributeDefinition> Attributes { get; } = new();
}
