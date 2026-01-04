namespace CodeGenerator.Domain.Artifacts;

/// <summary>
/// Base implementation for previewable decorators
/// </summary>
public abstract class PreviewableDecorator : ArtifactDecorator, IPreviewableDecorator
{
    public virtual bool CanPreview => true;
    public abstract object? CreatePreview();
}
