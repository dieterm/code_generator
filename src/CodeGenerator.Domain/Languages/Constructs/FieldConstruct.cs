namespace CodeGenerator.Domain.Languages.Constructs;

/// <summary>
/// Represents a field construct
/// </summary>
public class FieldConstruct
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public AccessModifier AccessModifier { get; set; } = AccessModifier.Private;
    public bool IsStatic { get; set; }
    public bool IsReadonly { get; set; }
    public bool IsConst { get; set; }
    public bool IsVolatile { get; set; }
    public string? DefaultValue { get; set; }
    public string? Documentation { get; set; }
    public List<AttributeDefinition> Attributes { get; } = new();
}
