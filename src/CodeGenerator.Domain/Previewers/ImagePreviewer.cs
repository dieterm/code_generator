namespace CodeGenerator.Domain.Previewers;

using CodeGenerator.Domain.Artifacts;
using CodeGenerator.Domain.Artifacts.FileSystem;

/// <summary>
/// Previewer for image files
/// </summary>
public class ImagePreviewer : IArtifactPreviewer
{
    public PreviewType PreviewType => PreviewType.Image;

    public bool CanPreview(Artifact artifact)
    {
        return artifact.HasDecorator<ImageFileDecorator>();
    }

    public object? CreatePreview(Artifact artifact)
    {
        var decorator = artifact.GetDecorator<ImageFileDecorator>();
        return decorator?.ImageData;
    }
}
