namespace CodeGenerator.Core.Models.Domain;

/// <summary>
/// Enum model
/// </summary>
public class EnumModel
{
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<EnumValue> Values { get; set; } = new();
    public string UnderlyingType { get; set; } = "int";
    public bool IsFlags { get; set; }
}
