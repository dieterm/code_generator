namespace CodeGenerator.Domain.Artifacts.FileSystem;

/// <summary>
/// Base decorator for file system artifacts
/// </summary>
public abstract class FileSystemDecorator : PreviewableDecorator
{
    /// <summary>
    /// Relative path from parent
    /// </summary>
    public string Path
    {
        get => Artifact?.GetProperty<string>("Path") ?? string.Empty;
        set => Artifact?.SetProperty("Path", value);
    }

    /// <summary>
    /// Full path including all parent paths
    /// </summary>
    public string FullPath
    {
        get
        {
            if (Artifact == null) return Path;
            
            var parentDecorator = Artifact.Parent?.GetDecorator<FileSystemDecorator>();
            if (parentDecorator != null)
            {
                var name = string.IsNullOrEmpty(Path) ? Artifact.Name : Path;
                return System.IO.Path.Combine(parentDecorator.FullPath, name);
            }
            return string.IsNullOrEmpty(Path) ? Artifact.Name : Path;
        }
    }
}
