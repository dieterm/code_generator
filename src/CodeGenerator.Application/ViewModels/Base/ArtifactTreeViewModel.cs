using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.Windows.Input;

namespace CodeGenerator.Application.ViewModels.Base;

/// <summary>
/// Base ViewModel for artifact tree views, providing common functionality
/// for context menus, selection, rename editing, and refresh
/// </summary>
public abstract class ArtifactTreeViewModel<TController> : ViewModelBase, IArtifactTreeViewModel where TController : IArtifactTreeViewController
{
    private IArtifact? _rootArtifact;
    private IArtifact? _selectedArtifact;

    /// <summary>
    /// Root artifact for the tree
    /// </summary>
    public IArtifact? RootArtifact
    {
        get => _rootArtifact;
        set => SetProperty(ref _rootArtifact, value);
    }

    /// <summary>
    /// Currently selected artifact in the tree
    /// </summary>
    public IArtifact? SelectedArtifact
    {
        get => _selectedArtifact;
        set
        {
            if (SetProperty(ref _selectedArtifact, value))
            {
                OnSelectedArtifactChanged(value);
            }
        }
    }

    public ICommand SelectArtifactCommand => new CodeGenerator.Shared.RelayCommand((artifact) => ArtifactSelected?.Invoke(this, artifact as IArtifact), (artifact) => artifact is IArtifact);

    /// <summary>
    /// Event raised when context menu commands are requested
    /// </summary>
    public event EventHandler<ArtifactContextMenuEventArgs>? ContextMenuRequested;
    /// <summary>
    /// Event raised when an artifact is selected in the treeview
    /// </summary>
    public event EventHandler<IArtifact>? ArtifactSelected;
    /// <summary>
    /// Event raised when an artifact needs to be refreshed in the tree
    /// </summary>
    public event EventHandler<IArtifact>? ArtifactRefreshRequested;
    /// <summary>
    /// Event raised when rename edit should be started for an artifact
    /// </summary>
    public event EventHandler<IArtifact>? BeginRenameRequested;

    protected TController TreeViewController { get; }

    protected ArtifactTreeViewModel(TController treeViewController)
    {
        TreeViewController = treeViewController;
    }

    /// <summary>
    /// Get context menu commands for an artifact
    /// </summary>
    public abstract IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact);

    /// <summary>
    /// Handle artifact double-click
    /// </summary>
    public abstract Task HandleDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when the selected artifact changes
    /// </summary>
    protected virtual void OnSelectedArtifactChanged(IArtifact? artifact)
    {
        // Override in derived classes if needed
    }

    /// <summary>
    /// Request context menu for an artifact
    /// </summary>
    public void RequestContextMenu(IArtifact artifact, int x, int y)
    {
        var commands = GetContextMenuCommands(artifact);
        ContextMenuRequested?.Invoke(this, new ArtifactContextMenuEventArgs(artifact, commands, x, y));
    }

    /// <summary>
    /// Request to refresh an artifact in the tree
    /// </summary>
    public void RequestArtifactRefresh(IArtifact artifact)
    {
        ArtifactRefreshRequested?.Invoke(this, artifact);
    }

    /// <summary>
    /// Cleanup resources
    /// </summary>
    public virtual void Cleanup()
    {
        // Override in derived classes if needed
    }
}
