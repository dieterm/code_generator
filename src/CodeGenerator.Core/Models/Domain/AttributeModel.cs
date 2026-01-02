using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Models.Domain;
/// <summary>
/// Attribute model for code generation
/// </summary>
public class AttributeModel
{
    /// <summary>
    /// Attribute name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Positional arguments
    /// </summary>
    public List<object> Arguments { get; set; } = new();

    /// <summary>
    /// Named arguments
    /// </summary>
    public Dictionary<string, object> NamedArguments { get; set; } = new();
}