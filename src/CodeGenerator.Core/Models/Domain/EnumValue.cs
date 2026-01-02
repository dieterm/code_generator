namespace CodeGenerator.Core.Models.Domain;

/// <summary>
/// Enum value for enum properties
/// </summary>
public class EnumValue
{
    public string Name { get; set; } = string.Empty;
    public object Value { get; set; } = 0;
    public string? Description { get; set; }
}
