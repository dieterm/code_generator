namespace CodeGenerator.Domain.Artifacts;

/// <summary>
/// Interface for decorators that can be previewed
/// </summary>
public interface IPreviewableDecorator : IArtifactDecorator
{
    /// <summary>
    /// Whether this decorator can create a preview
    /// </summary>
    bool CanPreview { get; }

    /// <summary>
    /// Create a preview representation
    /// </summary>
    object? CreatePreview();
}
