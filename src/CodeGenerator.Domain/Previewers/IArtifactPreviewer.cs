namespace CodeGenerator.Domain.Previewers;

using CodeGenerator.Domain.Artifacts;

/// <summary>
/// Interface for components that can create previews of artifacts
/// </summary>
public interface IArtifactPreviewer
{
    /// <summary>
    /// Determines whether this previewer can handle the given artifact
    /// </summary>
    bool CanPreview(Artifact artifact);

    /// <summary>
    /// Creates a preview for the given artifact
    /// </summary>
    object? CreatePreview(Artifact artifact);

    /// <summary>
    /// The type of preview this previewer creates
    /// </summary>
    PreviewType PreviewType { get; }
}
