namespace CodeGenerator.Domain.Previewers;

using CodeGenerator.Domain.Artifacts;
using CodeGenerator.Domain.Artifacts.FileSystem;

/// <summary>
/// Previewer for folder structures
/// </summary>
public class FolderStructurePreviewer : IArtifactPreviewer
{
    public PreviewType PreviewType => PreviewType.FolderStructure;

    public bool CanPreview(Artifact artifact)
    {
        return artifact.HasDecorator<DirectoryDecorator>();
    }

    public object? CreatePreview(Artifact artifact)
    {
        if (artifact.HasDecorator<DirectoryDecorator>())
        {
            return new DirectoryPreviewModel(artifact);
        }
        return null;
    }
}
