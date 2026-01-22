using CodeGenerator.Shared.Memento;
using System.ComponentModel;

namespace CodeGenerator.Core.Artifacts;

/// <summary>
/// Interface for artifact decorators that determine what an artifact represents
/// </summary>
public interface IArtifactDecorator : IMementoObject
{
    string Key { get; }
    /// <summary>
    /// The artifact this decorator is attached to
    /// </summary>
    Artifact? Artifact { get; }

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
