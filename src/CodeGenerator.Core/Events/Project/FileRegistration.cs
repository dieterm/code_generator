namespace CodeGenerator.Core.Events;

/// <summary>
/// Information about a file being registered/created
/// </summary>
public class FileRegistration
{
    /// <summary>
    /// filename only without path (e.g. "CustomerService.cs")
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>
    /// relative folderpath without filename (e.g. "Services/Customer")
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? TemplateName { get; set; }
    public string RegisteredBy { get; set; } = string.Empty;
    public bool OverwriteExisting { get; set; } = true;
}
