namespace CodeGenerator.Core.Workspaces.Datasources.Directory.Models;

/// <summary>
/// Model representing a file in a directory for template use
/// </summary>
public class FileModel
{
    private string? _content;
    private bool _contentLoaded;

    /// <summary>
    /// File name without extension
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// File extension including the dot (e.g., ".cs")
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// Full file name (Title + Extension)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Full path to the file
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// Directory containing the file
    /// </summary>
    public string Directory { get; set; } = string.Empty;

    /// <summary>
    /// Relative path from the root directory
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// File creation date/time
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// File last modified date/time
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// File last accessed date/time
    /// </summary>
    public DateTime LastAccessDate { get; set; }

    /// <summary>
    /// Whether the file is read-only
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Whether the file is hidden
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Whether the file is a system file
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// File attributes as string
    /// </summary>
    public string Attributes { get; set; } = string.Empty;

    /// <summary>
    /// File content as string (lazy loaded on first access)
    /// </summary>
    public string Content
    {
        get
        {
            if (!_contentLoaded)
            {
                _contentLoaded = true;
                try
                {
                    if (File.Exists(FullPath))
                    {
                        _content = File.ReadAllText(FullPath);
                    }
                }
                catch
                {
                    _content = string.Empty;
                }
            }
            return _content ?? string.Empty;
        }
    }

    /// <summary>
    /// Create a FileModel from a FileInfo object
    /// </summary>
    public static FileModel FromFileInfo(FileInfo fileInfo, string rootDirectory)
    {
        return new FileModel
        {
            Title = Path.GetFileNameWithoutExtension(fileInfo.Name),
            Extension = fileInfo.Extension,
            Name = fileInfo.Name,
            FullPath = fileInfo.FullName,
            Directory = fileInfo.DirectoryName ?? string.Empty,
            RelativePath = Path.GetRelativePath(rootDirectory, fileInfo.FullName),
            Size = fileInfo.Length,
            CreationDate = fileInfo.CreationTime,
            ModifiedDate = fileInfo.LastWriteTime,
            LastAccessDate = fileInfo.LastAccessTime,
            IsReadOnly = fileInfo.IsReadOnly,
            IsHidden = (fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden,
            IsSystem = (fileInfo.Attributes & FileAttributes.System) == FileAttributes.System,
            Attributes = fileInfo.Attributes.ToString()
        };
    }
}
