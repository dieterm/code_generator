namespace CodeGenerator.Core.Workspaces.Datasources.Directory.Models;

/// <summary>
/// Model representing a directory for template use
/// </summary>
public class DirectoryModel
{
    /// <summary>
    /// Directory name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Full path to the directory
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// Relative path from the root directory
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>
    /// Parent directory path
    /// </summary>
    public string ParentDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Directory creation date/time
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Directory last modified date/time
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// Whether the directory is hidden
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Whether the directory is a system directory
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Directory attributes as string
    /// </summary>
    public string Attributes { get; set; } = string.Empty;

    /// <summary>
    /// Total size of all files in this directory (not including subdirectories)
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Number of files directly in this directory
    /// </summary>
    public int FileCount { get; set; }

    /// <summary>
    /// Number of subdirectories directly in this directory
    /// </summary>
    public int DirectoryCount { get; set; }

    /// <summary>
    /// Files in this directory
    /// </summary>
    public List<FileModel> Files { get; set; } = new();

    /// <summary>
    /// Subdirectories in this directory
    /// </summary>
    public List<DirectoryModel> Directories { get; set; } = new();

    /// <summary>
    /// Create a DirectoryModel from a DirectoryInfo object
    /// </summary>
    public static DirectoryModel FromDirectoryInfo(DirectoryInfo directoryInfo, string rootDirectory, string? searchPattern = null, bool recursive = true)
    {
        var model = new DirectoryModel
        {
            Name = directoryInfo.Name,
            FullPath = directoryInfo.FullName,
            RelativePath = Path.GetRelativePath(rootDirectory, directoryInfo.FullName),
            ParentDirectory = directoryInfo.Parent?.FullName ?? string.Empty,
            CreationDate = directoryInfo.CreationTime,
            ModifiedDate = directoryInfo.LastWriteTime,
            IsHidden = (directoryInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden,
            IsSystem = (directoryInfo.Attributes & FileAttributes.System) == FileAttributes.System,
            Attributes = directoryInfo.Attributes.ToString()
        };

        // Add files
        try
        {
            var pattern = string.IsNullOrEmpty(searchPattern) ? "*.*" : searchPattern;
            foreach (var file in directoryInfo.GetFiles(pattern))
            {
                model.Files.Add(FileModel.FromFileInfo(file, rootDirectory));
                model.Size += file.Length;
            }
            model.FileCount = model.Files.Count;
        }
        catch (UnauthorizedAccessException)
        {
            // Skip directories we don't have access to
        }
        catch (DirectoryNotFoundException)
        {
            // Directory was deleted during enumeration
        }

        // Add subdirectories recursively
        if (recursive)
        {
            try
            {
                foreach (var subDir in directoryInfo.GetDirectories())
                {
                    model.Directories.Add(FromDirectoryInfo(subDir, rootDirectory, searchPattern, recursive));
                }
                model.DirectoryCount = model.Directories.Count;
            }
            catch (UnauthorizedAccessException)
            {
                // Skip directories we don't have access to
            }
            catch (DirectoryNotFoundException)
            {
                // Directory was deleted during enumeration
            }
        }

        return model;
    }
}
