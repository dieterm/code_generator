using CodeGenerator.Core.Enums;

namespace CodeGenerator.Core.Models.Configuration;

/// <summary>
/// Naming convention settings
/// </summary>
public class NamingConventions
{
    public string ClassNaming { get; set; } = "PascalCase";
    public string PropertyNaming { get; set; } = "PascalCase";
    public string FieldNaming { get; set; } = "camelCase";
    public string MethodNaming { get; set; } = "PascalCase";
    public string ConstantNaming { get; set; } = "UPPER_CASE";
    public string InterfacePrefix { get; set; } = "I";
    public string PrivateFieldPrefix { get; set; } = "_";
}