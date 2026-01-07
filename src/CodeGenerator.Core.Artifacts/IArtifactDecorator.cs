namespace CodeGenerator.Core.Artifacts;

/// <summary>
/// Interface for artifact decorators that determine what an artifact represents
/// </summary>
public interface IArtifactDecorator
{
    string Key { get; }
    /// <summary>
    /// The artifact this decorator is attached to
    /// </summary>
    Artifact? Artifact { get; }

    /// <summary>
    /// Priority for ordering decorators (lower = higher priority)
    /// </summary>
    //int Priority { get; set; }

    /// <summary>
    /// Identifier of the component that registered this decorator
    /// </summary>
    //string RegisteredBy { get; set; }

    /// <summary>
    /// Attach this decorator to an artifact
    /// </summary>
    void Attach(Artifact artifact);

    /// <summary>
    /// Detach this decorator from its artifact
    /// </summary>
    void Detach();

    bool CanGenerate();
    Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default);
}
