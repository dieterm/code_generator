namespace CodeGenerator.Domain.Artifacts.FileSystem;

using CodeGenerator.Domain.Previewers;

/// <summary>
/// Decorator that marks an artifact as a directory
/// </summary>
public class DirectoryDecorator : FileSystemDecorator
{
    /// <summary>
    /// Files in this directory (children with FileDecorator)
    /// </summary>
    public IEnumerable<Artifact> Files => 
        Artifact?.Children.Where(c => c.HasDecorator<FileDecorator>()) ?? Enumerable.Empty<Artifact>();

    /// <summary>
    /// Subdirectories in this directory (children with DirectoryDecorator)
    /// </summary>
    public IEnumerable<Artifact> Subdirectories => 
        Artifact?.Children.Where(c => c.HasDecorator<DirectoryDecorator>()) ?? Enumerable.Empty<Artifact>();

    /// <summary>
    /// Total count of files including subdirectories
    /// </summary>
    public int TotalFileCount
    {
        get
        {
            var count = Files.Count();
            foreach (var subdir in Subdirectories)
            {
                var subdirDecorator = subdir.GetDecorator<DirectoryDecorator>();
                if (subdirDecorator != null)
                    count += subdirDecorator.TotalFileCount;
            }
            return count;
        }
    }

    public override object? CreatePreview()
    {
        if (Artifact == null) return null;
        return new DirectoryPreviewModel(Artifact);
    }

    /// <summary>
    /// Add a file to this directory
    /// </summary>
    public Artifact AddFile(string name, string content, string extension = "")
    {
        var file = new Artifact { Name = name };
        var fileDecorator = new FileDecorator
        {
            Extension = extension,
            Content = content
        };
        file.AddDecorator(fileDecorator);
        Artifact?.AddChild(file);
        return file;
    }

    /// <summary>
    /// Add a subdirectory
    /// </summary>
    public Artifact AddSubdirectory(string name)
    {
        var dir = new Artifact { Name = name };
        dir.AddDecorator(new DirectoryDecorator());
        Artifact?.AddChild(dir);
        return dir;
    }

    /// <summary>
    /// Get or create a subdirectory by name
    /// </summary>
    public Artifact GetOrCreateSubdirectory(string name)
    {
        var existing = Subdirectories.FirstOrDefault(d => 
            d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        
        if (existing != null)
            return existing;

        return AddSubdirectory(name);
    }
}
