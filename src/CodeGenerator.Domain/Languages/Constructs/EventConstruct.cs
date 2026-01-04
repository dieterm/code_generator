namespace CodeGenerator.Domain.Languages.Constructs;

/// <summary>
/// Represents an event construct
/// </summary>
public class EventConstruct
{
    public string Name { get; set; } = string.Empty;
    public string EventHandlerType { get; set; } = "EventHandler";
    public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
    public bool IsStatic { get; set; }
    public bool IsVirtual { get; set; }
    public bool IsOverride { get; set; }
    public bool IsAbstract { get; set; }
    public string? Documentation { get; set; }
    public List<AttributeDefinition> Attributes { get; } = new();
}
