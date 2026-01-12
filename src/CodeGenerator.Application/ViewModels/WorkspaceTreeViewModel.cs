using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Application.ViewModels
{
    /// <summary>
    /// ViewModel for the WorkspaceTreeView
    /// </summary>
    public class WorkspaceTreeViewModel : ViewModelBase
    {
        private readonly WorkspaceController _workspaceController;
        private WorkspaceArtifact? _workspace;
        private IArtifact? _selectedArtifact;
        private bool _isDirty;

        public WorkspaceTreeViewModel(WorkspaceController workspaceController)
        {
            _workspaceController = workspaceController;
            _workspaceController.WorkspaceChanged += OnWorkspaceChanged;
            _workspaceController.BeginRenameRequested += OnBeginRenameRequested;
            _workspaceController.ArtifactPropertyChanged += OnArtifactPropertyChanged;
        }

        /// <summary>
        /// The current workspace
        /// </summary>
        public WorkspaceArtifact? Workspace
        {
            get => _workspace;
            private set => SetProperty(ref _workspace, value);
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
                    //OnSelectedArtifactChanged();
                }
            }
        }

        /// <summary>
        /// Indicates if the workspace has unsaved changes
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            private set => SetProperty(ref _isDirty, value);
        }

        /// <summary>
        /// Event raised when context menu commands are requested
        /// </summary>
        public event EventHandler<ContextMenuRequestedEventArgs>? ContextMenuRequested;

        /// <summary>
        /// Event raised when a detail view should be shown
        /// </summary>
        //public event EventHandler<DetailViewRequestedEventArgs>? DetailViewRequested;

        /// <summary>
        /// Event raised when rename edit should be started for an artifact
        /// </summary>
        public event EventHandler<IArtifact>? BeginRenameRequested;

        /// <summary>
        /// Event raised when an artifact needs to be refreshed in the tree
        /// </summary>
        public event EventHandler<IArtifact>? ArtifactRefreshRequested;

        /// <summary>
        /// Get context menu commands for an artifact
        /// </summary>
        public IEnumerable<WorkspaceCommand> GetContextMenuCommands(IArtifact artifact)
        {
            return _workspaceController.GetContextMenuCommands(artifact);
        }

        /// <summary>
        /// Request context menu for an artifact
        /// </summary>
        public void RequestContextMenu(IArtifact artifact, int x, int y)
        {
            var commands = GetContextMenuCommands(artifact);
            ContextMenuRequested?.Invoke(this, new ContextMenuRequestedEventArgs(artifact, commands, x, y));
        }

        /// <summary>
        /// Handle artifact double-click
        /// </summary>
        public async Task HandleDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default)
        {
            var controller = _workspaceController.GetController(artifact);
            if (controller != null)
            {
                await controller.OnDoubleClickAsync(artifact, cancellationToken);
            }
        }

        /// <summary>
        /// Handle artifact rename from the tree view
        /// </summary>
        public void OnArtifactRenamed(IArtifact artifact, string oldName, string newName)
        {
            if (oldName != newName)
            {
                IsDirty = true;
                
                // Notify the controller about the rename
                _workspaceController.OnArtifactRenamed(artifact, oldName, newName);
            }
        }

        private void OnWorkspaceChanged(object? sender, WorkspaceArtifact? workspace)
        {
            Workspace = workspace;
            SelectedArtifact = workspace;
            IsDirty = false;
        }

        private void OnBeginRenameRequested(object? sender, IArtifact artifact)
        {
            // Forward the event to the view
            BeginRenameRequested?.Invoke(this, artifact);
        }

        private void OnArtifactPropertyChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            // Mark as dirty
            IsDirty = true;
            
            // Request refresh of the artifact in the tree
            ArtifactRefreshRequested?.Invoke(this, e.Artifact);
        }

        //private async void OnSelectedArtifactChanged()
        //{
        //    if (_selectedArtifact != null)
        //    {
        //        await _workspaceController.SelectArtifactAsync(_selectedArtifact);

        //        var controller = _workspaceController.GetController(_selectedArtifact);
        //        var detailView = controller?.CreateDetailView(_selectedArtifact);
        //        DetailViewRequested?.Invoke(this, new DetailViewRequestedEventArgs(_selectedArtifact, detailView));
        //    }
        //}

        public void Cleanup()
        {
            _workspaceController.WorkspaceChanged -= OnWorkspaceChanged;
            _workspaceController.BeginRenameRequested -= OnBeginRenameRequested;
            _workspaceController.ArtifactPropertyChanged -= OnArtifactPropertyChanged;
        }
    }

    /// <summary>
    /// Event args for context menu request
    /// </summary>
    public class ContextMenuRequestedEventArgs : EventArgs
    {
        public IArtifact Artifact { get; }
        public IEnumerable<WorkspaceCommand> Commands { get; }
        public int X { get; }
        public int Y { get; }

        public ContextMenuRequestedEventArgs(IArtifact artifact, IEnumerable<WorkspaceCommand> commands, int x, int y)
        {
            Artifact = artifact;
            Commands = commands;
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Event args for detail view request
    /// </summary>
    public class DetailViewRequestedEventArgs : EventArgs
    {
        public IArtifact Artifact { get; }
        public object? DetailView { get; }

        public DetailViewRequestedEventArgs(IArtifact artifact, object? detailView)
        {
            Artifact = artifact;
            DetailView = detailView;
        }
    }
}
