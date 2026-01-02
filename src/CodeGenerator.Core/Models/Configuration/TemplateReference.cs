using CodeGenerator.Core.Enums;

namespace CodeGenerator.Core.Models.Configuration;

/// <summary>
/// Reference to a template file
/// </summary>
public class TemplateReference
{
    /// <summary>
    /// Template identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Template file name (relative to template folder)
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Output file name pattern
    /// </summary>
    public string OutputFileName { get; set; } = string.Empty;

    /// <summary>
    /// Whether this template generates one file per entity
    /// </summary>
    public bool PerEntity { get; set; } = true;

    /// <summary>
    /// Condition for when to use this template
    /// </summary>
    public string? Condition { get; set; }
}
