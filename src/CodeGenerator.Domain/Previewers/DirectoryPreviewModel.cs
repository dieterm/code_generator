namespace CodeGenerator.Domain.Previewers;

using CodeGenerator.Domain.Artifacts;
using CodeGenerator.Domain.Artifacts.FileSystem;

/// <summary>
/// Model for directory preview
/// </summary>
public class DirectoryPreviewModel
{
    public string Name { get; }
    public string FullPath { get; }
    public List<DirectoryPreviewModel> Subdirectories { get; } = new();
    public List<FilePreviewModel> Files { get; } = new();

    public DirectoryPreviewModel(Artifact artifact)
    {
        var decorator = artifact.GetDecorator<DirectoryDecorator>();
        Name = artifact.Name;
        FullPath = decorator?.FullPath ?? artifact.Name;

        foreach (var subdir in artifact.Children.Where(c => c.HasDecorator<DirectoryDecorator>()))
        {
            Subdirectories.Add(new DirectoryPreviewModel(subdir));
        }

        foreach (var file in artifact.Children.Where(c => c.HasDecorator<FileDecorator>() && !c.HasDecorator<DirectoryDecorator>()))
        {
            Files.Add(new FilePreviewModel(file));
        }
    }
}

/// <summary>
/// Model for file preview
/// </summary>
public class FilePreviewModel
{
    public string Name { get; }
    public string Extension { get; }
    public long Size { get; }

    public FilePreviewModel(Artifact artifact)
    {
        var decorator = artifact.GetDecorator<FileDecorator>();
        Name = decorator?.FileName ?? artifact.Name;
        Extension = decorator?.Extension ?? string.Empty;
        Size = decorator?.Size ?? 0;
    }
}
