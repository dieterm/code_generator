namespace CodeGenerator.Domain.Previewers;

using CodeGenerator.Domain.Artifacts;
using CodeGenerator.Domain.Artifacts.DotNet;

/// <summary>
/// Previewer for .NET projects
/// </summary>
public class ProjectPreviewer : IArtifactPreviewer
{
    public PreviewType PreviewType => PreviewType.Project;

    public bool CanPreview(Artifact artifact)
    {
        return artifact.HasDecorator<DotNetProjectDecorator>();
    }

    public object? CreatePreview(Artifact artifact)
    {
        if (artifact.HasDecorator<DotNetProjectDecorator>())
        {
            return new ProjectPreviewModel(artifact);
        }
        return null;
    }
}
