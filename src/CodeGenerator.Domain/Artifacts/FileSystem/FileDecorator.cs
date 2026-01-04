namespace CodeGenerator.Domain.Artifacts.FileSystem;

/// <summary>
/// Decorator that marks an artifact as a file
/// </summary>
public class FileDecorator : FileSystemDecorator
{
    /// <summary>
    /// File extension (including the dot, e.g., ".cs")
    /// </summary>
    public string Extension
    {
        get => Artifact?.GetProperty<string>("Extension") ?? string.Empty;
        set => Artifact?.SetProperty("Extension", value);
    }

    /// <summary>
    /// File content
    /// </summary>
    public string Content
    {
        get => Artifact?.GetProperty<string>("Content") ?? string.Empty;
        set => Artifact?.SetProperty("Content", value);
    }

    /// <summary>
    /// File size in bytes (based on content length)
    /// </summary>
    public long Size => Content?.Length ?? 0;

    /// <summary>
    /// Full file name including extension
    /// </summary>
    public string FileName => Artifact != null
        ? (string.IsNullOrEmpty(Extension) ? Artifact.Name : $"{Artifact.Name}{Extension}")
        : string.Empty;

    public override object? CreatePreview() => Content;
}
