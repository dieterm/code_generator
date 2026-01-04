namespace CodeGenerator.Domain.Languages.Constructs;

/// <summary>
/// Represents a property construct
/// </summary>
public class PropertyConstruct
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
    public bool HasGetter { get; set; } = true;
    public bool HasSetter { get; set; } = true;
    public AccessModifier? GetterAccessModifier { get; set; }
    public AccessModifier? SetterAccessModifier { get; set; }
    public bool IsStatic { get; set; }
    public bool IsVirtual { get; set; }
    public bool IsOverride { get; set; }
    public bool IsAbstract { get; set; }
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? Documentation { get; set; }
    public List<AttributeDefinition> Attributes { get; } = new();
}
