using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Application.Controllers.Base;

/// <summary>
/// Service for managing artifact clipboard operations.
/// Uses an internal clipboard rather than the system clipboard for security and type safety.
/// </summary>
public class ArtifactClipboardService
{
    private static ArtifactClipboardService? _instance;
    private static readonly object _lock = new();

    /// <summary>
    /// Singleton instance of the clipboard service
    /// </summary>
    public static ArtifactClipboardService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new ArtifactClipboardService();
                }
            }
            return _instance;
        }
    }

    private ArtifactClipboardData? _clipboardData;

    /// <summary>
    /// Event raised when clipboard content changes
    /// </summary>
    public event EventHandler? ClipboardChanged;

    /// <summary>
    /// Check if there is data on the clipboard
    /// </summary>
    public bool HasData => _clipboardData != null;

    /// <summary>
    /// Get the current clipboard data
    /// </summary>
    public ArtifactClipboardData? GetData()
    {
        return _clipboardData;
    }

    /// <summary>
    /// Get the artifact from clipboard if still available
    /// </summary>
    public IArtifact? GetArtifact()
    {
        if (_clipboardData?.SourceArtifact != null && 
            _clipboardData.SourceArtifact.TryGetTarget(out var artifact))
        {
            return artifact;
        }
        return null;
    }

    /// <summary>
    /// Copy an artifact to the clipboard
    /// </summary>
    public void Copy(IArtifact artifact)
    {
        SetClipboardData(artifact, ClipboardOperation.Copy);
    }

    /// <summary>
    /// Cut an artifact to the clipboard
    /// </summary>
    public void Cut(IArtifact artifact)
    {
        SetClipboardData(artifact, ClipboardOperation.Cut);
    }

    /// <summary>
    /// Clear the clipboard
    /// </summary>
    public void Clear()
    {
        _clipboardData = null;
        ClipboardChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Clear the clipboard if it contains the specified artifact (used after paste of a cut operation)
    /// </summary>
    public void ClearIfContains(IArtifact artifact)
    {
        if (_clipboardData != null && _clipboardData.ArtifactId == artifact.Id)
        {
            Clear();
        }
    }

    /// <summary>
    /// Check if the clipboard contains a cut operation
    /// </summary>
    public bool IsCutOperation => _clipboardData?.Operation == ClipboardOperation.Cut;

    /// <summary>
    /// Check if the clipboard contains a copy operation
    /// </summary>
    public bool IsCopyOperation => _clipboardData?.Operation == ClipboardOperation.Copy;

    private void SetClipboardData(IArtifact artifact, ClipboardOperation operation)
    {
        _clipboardData = new ArtifactClipboardData
        {
            ArtifactId = artifact.Id,
            ArtifactTypeName = artifact.GetType().FullName ?? artifact.GetType().Name,
            Operation = operation,
            Timestamp = DateTime.UtcNow,
            SourceArtifact = new WeakReference<IArtifact>(artifact)
        };

        ClipboardChanged?.Invoke(this, EventArgs.Empty);
    }
}
