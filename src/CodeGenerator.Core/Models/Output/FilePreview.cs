namespace CodeGenerator.Core.Models.Output;

/// <summary>
/// Preview of a file to be generated
/// </summary>
public class FilePreview
{
    public string RelativePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string GeneratorId { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public bool WillOverwrite { get; set; }

    /// <summary>
    /// Whether this file can be previewed as a UserControl
    /// </summary>
    public bool IsPreviewable =>
        GeneratorId == "View_WinForms" &&
        !FileName.EndsWith(".Designer.cs") &&
        FileName.EndsWith(".cs");

    /// <summary>
    /// Related files needed for compilation (e.g., Designer.cs)
    /// </summary>
    public List<FilePreview>? RelatedFiles { get; set; }
}
