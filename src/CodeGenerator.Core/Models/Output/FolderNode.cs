namespace CodeGenerator.Core.Models.Output;

/// <summary>
/// Folder node for tree representation
/// </summary>
public class FolderNode
{
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public List<FolderNode> Folders { get; set; } = new();
    public List<FilePreview> Files { get; set; } = new();
    public bool IsProject { get; set; }
    public string? ProjectType { get; set; }
}
