namespace CodeGenerator.Core.Models.Output;

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
    /// Generated content
    /// </summary>
    public string Content { get; set; } = string.Empty;

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

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long Size => Content?.Length ?? 0;

    /// <summary>
    /// Whether this file can be previewed as a UserControl
    /// </summary>
    public bool IsPreviewable => 
        GeneratorId == "View_WinForms" && 
        !FileName.EndsWith(".Designer.cs") &&
        FileName.EndsWith(".cs");

    /// <summary>
    /// Additional files needed for compilation (e.g., Designer.cs for WinForms)
    /// </summary>
    public List<string>? RelatedFiles { get; set; }
}
