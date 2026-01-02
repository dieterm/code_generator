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
}
