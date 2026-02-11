using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Application.Controllers.Base;

/// <summary>
/// Represents clipboard data for artifact cut/copy operations
/// </summary>
public class ArtifactClipboardData
{
    /// <summary>
    /// Unique identifier for the artifact on the clipboard
    /// </summary>
    public string ArtifactId { get; init; } = string.Empty;

    /// <summary>
    /// The type of the artifact
    /// </summary>
    public string ArtifactTypeName { get; init; } = string.Empty;

    /// <summary>
    /// The operation that was performed (Copy or Cut)
    /// </summary>
    public ClipboardOperation Operation { get; init; }

    /// <summary>
    /// Timestamp when the data was placed on clipboard
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Reference to the original artifact (if still available)
    /// </summary>
    public WeakReference<IArtifact>? SourceArtifact { get; init; }
}

/// <summary>
/// Type of clipboard operation
/// </summary>
public enum ClipboardOperation
{
    Copy,
    Cut
}
