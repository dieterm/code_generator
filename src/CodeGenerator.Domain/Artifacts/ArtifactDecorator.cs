namespace CodeGenerator.Domain.Artifacts;

/// <summary>
/// Base implementation of artifact decorator
/// </summary>
public abstract class ArtifactDecorator : IArtifactDecorator
{
    public Artifact? Artifact { get; private set; }
    public int Priority { get; set; } = 100;
    public string RegisteredBy { get; set; } = string.Empty;

    public void Attach(Artifact artifact)
    {
        Artifact = artifact;
        OnAttached();
    }

    public void Detach()
    {
        OnDetaching();
        Artifact = null;
    }

    /// <summary>
    /// Called when the decorator is attached to an artifact
    /// </summary>
    protected virtual void OnAttached() { }

    /// <summary>
    /// Called when the decorator is about to be detached
    /// </summary>
    protected virtual void OnDetaching() { }
}
