using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Base;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Shared.ViewModels;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Base;

/// <summary>
/// Abstract base controller for managing artifact trees
/// Coordinates between artifact controllers and the UI
/// </summary>
public abstract class ArtifactTreeViewController<TTreeViewModel> : IArtifactTreeViewController where TTreeViewModel : class, IArtifactTreeViewModel
{
    /// <summary>
    /// Event raised when an artifact is renamed
    /// </summary>
    public event EventHandler<ArtifactRenamedEventArgs>? ArtifactRenamed;
    /// <summary>
    /// Event raised when an artifact is selected
    /// </summary>
    public event EventHandler<IArtifact?>? ArtifactSelected;
    /// <summary>
    /// Event raised when an artifact property changes (e.g., from edit view)
    /// </summary>
    public event EventHandler<ArtifactPropertyChangedEventArgs>? ArtifactPropertyChanged;

    /// <summary>
    /// Event raised when a child artifact is added
    /// </summary>
    public event EventHandler<ArtifactChildChangedEventArgs>? ArtifactAdded;
    /// <summary>
    /// Event raised when a rename edit should be started for an artifact
    /// </summary>
    public event EventHandler<IArtifact>? BeginRenameRequested;
    /// <summary>
    /// Event raised when a child artifact is removed
    /// </summary>
    public event EventHandler<ArtifactChildChangedEventArgs>? ArtifactRemoved;
    /// <summary>
    /// Notify that an artifact property has changed
    /// </summary>
    public void OnArtifactPropertyChanged(IArtifact artifact, string propertyName, object? newValue)
    {
        Logger.LogDebug("Artifact property changed: {PropertyName} = {NewValue}", propertyName, newValue);
        ArtifactPropertyChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(artifact, propertyName, newValue));
    }

    /// <summary>
    /// Notify that a child artifact was added
    /// </summary>
    public void OnArtifactAdded(IArtifact parent, IArtifact child)
    {
        Logger.LogDebug("Artifact added: {ChildId} to parent {ParentId}", child.Id, parent.Id);
        ArtifactAdded?.Invoke(this, new ArtifactChildChangedEventArgs(parent, child));
    }

    /// <summary>
    /// Notify that a child artifact was removed
    /// </summary>
    public void OnArtifactRemoved(IArtifact parent, IArtifact child)
    {
        Logger.LogDebug("Artifact removed: {ChildId} from parent {ParentId}", child.Id, parent.Id);
        ArtifactRemoved?.Invoke(this, new ArtifactChildChangedEventArgs(parent, child));
    }

    /// <summary>
    /// Request the UI to begin rename editing for an artifact
    /// </summary>
    public void RequestBeginRename(IArtifact artifact)
    {
        Logger.LogDebug("Requesting rename for artifact: {Id}", artifact.Id);
        BeginRenameRequested?.Invoke(this, artifact);
    }
    private List<IArtifactController>? _artifactControllers;

    protected ILogger Logger { get; }
    protected IWindowManagerService WindowManagerService { get; }
    protected IMessageBoxService MessageBoxService { get; }

    protected ArtifactTreeViewController(
        IWindowManagerService windowManagerService,
        IMessageBoxService messageBoxService,
        ILogger logger)
    {
        WindowManagerService = windowManagerService;
        MessageBoxService = messageBoxService;
        Logger = logger;
    }

    /// <summary>
    /// Get all registered artifact controllers (lazy loaded)
    /// </summary>
    protected List<IArtifactController> ArtifactControllers
    {
        get
        {
            if (_artifactControllers == null)
            {
                _artifactControllers = LoadArtifactControllers().ToList();
                Logger.LogDebug("Loaded {Count} artifact controllers", _artifactControllers.Count);
            }
            return _artifactControllers;
        }
    }

    /// <summary>
    /// Load artifact controllers - override in derived classes to provide controllers
    /// </summary>
    protected abstract IEnumerable<IArtifactController> LoadArtifactControllers();

    private TTreeViewModel? _treeViewModel;
    public TTreeViewModel? TreeViewModel
    {
        get { return _treeViewModel ??= CreateTreeViewModel(); }
        set { _treeViewModel = value; }
    }

    IArtifactTreeViewModel? IArtifactTreeViewController.TreeViewModel { 
        get => TreeViewModel; 
        set => TreeViewModel = (TTreeViewModel?)value;
    }

    protected TTreeViewModel CreateTreeViewModel()
    {
        var viewModel = Activator.CreateInstance(typeof(TTreeViewModel), this) as TTreeViewModel;
        if (viewModel == null)
        {
            throw new InvalidOperationException($"Could not create instance of {typeof(TTreeViewModel).FullName}");
        }
        viewModel.ArtifactSelected += ViewModel_ArtifactSelected;
        return viewModel;
    }

    private async void ViewModel_ArtifactSelected(object? sender, IArtifact e)
    {
        await SelectArtifactAsync(e);
    }

    public abstract void ShowArtifactDetailsView(ViewModelBase? detailsModel);
   

    /// <summary>
    /// Get the controller for an artifact
    /// </summary>
    public IArtifactController? GetController(IArtifact artifact)
    {
        return ArtifactControllers.FirstOrDefault(c => c.CanHandle(artifact));
    }

    /// <summary>
    /// Get context menu commands for an artifact
    /// </summary>
    public IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact)
    {
        var controller = GetController(artifact);
        if (controller != null)
        {
            return controller.GetContextMenuCommands(artifact);
        }
        return Enumerable.Empty<ArtifactTreeNodeCommand>();
    }

    /// <summary>
    /// Handle artifact selection
    /// </summary>
    public async Task SelectArtifactAsync(IArtifact artifact, CancellationToken cancellationToken = default)
    {
        TreeViewModel!.SelectedArtifact = artifact;

        OnArtifactSelected(artifact);

        var controller = GetController(artifact);
        if (controller != null)
        {
            await controller.OnSelectedAsync(artifact, cancellationToken);
        }
    }

    /// <summary>
    /// Called when an artifact is selected - override in derived classes if needed
    /// This raises the ArtifactSelected event
    /// </summary>
    protected virtual void OnArtifactSelected(IArtifact artifact)
    {
        ArtifactSelected?.Invoke(this, artifact);
    }

    /// <summary>
    /// Handle artifact double-click
    /// </summary>
    public async Task HandleDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default)
    {
        var controller = GetController(artifact);
        if (controller != null)
        {
            await controller.OnDoubleClickAsync(artifact, cancellationToken);
        }
    }

    /// <summary>
    /// Handle artifact rename
    /// </summary>
    public virtual void OnArtifactRenamed(IArtifact artifact, string oldName, string newName)
    {
        ArtifactRenamed?.Invoke(this, new ArtifactRenamedEventArgs(artifact, oldName, newName));

        var controller = GetController(artifact);
        if (controller != null)
        {
            controller.OnArtifactRenamed(artifact, oldName, newName);
        }

        Logger.LogDebug("Artifact renamed: '{OldName}' -> '{NewName}'", oldName, newName);
    }

}
