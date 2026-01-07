namespace CodeGenerator.Generators.DotNet.Models;

/// <summary>
/// Represents a generated file
/// </summary>
public class GeneratedFile
{
    /// <summary>
    /// Relative path from output root
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>
    /// Absolute path
    /// </summary>
    public string AbsolutePath { get; set; } = string.Empty;

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Generator that created this file
    /// </summary>
    public string GeneratorId { get; set; } = string.Empty;

    /// <summary>
    /// Template used
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// Entity this file was generated for (if applicable)
    /// </summary>
    public string? EntityName { get; set; }

    /// <summary>
    /// Whether this is a new file or an update
    /// </summary>
    public bool IsNew { get; set; } = true;

    /// <summary>
    /// Whether the file was actually written to disk
    /// </summary>
    public bool Written { get; set; }
}
