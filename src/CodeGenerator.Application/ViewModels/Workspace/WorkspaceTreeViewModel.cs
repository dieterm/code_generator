using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.ViewModels.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;

namespace CodeGenerator.Application.ViewModels.Workspace
{
    /// <summary>
    /// ViewModel for the WorkspaceTreeView
    /// </summary>
    public class WorkspaceTreeViewModel : ArtifactTreeViewModel<WorkspaceTreeViewController>
    {
        //private readonly WorkspaceTreeViewController TreeViewController;
        private WorkspaceArtifact? _workspace;
        private bool _isDirty;

        public WorkspaceTreeViewModel(WorkspaceTreeViewController workspaceController)
            : base(workspaceController) 
        {
            
            //TreeViewController = workspaceController;
            TreeViewController.WorkspaceChanged += OnWorkspaceChanged;
            //_workspaceController.BeginRenameRequested += OnBeginRenameRequested;
            //_workspaceController.ArtifactPropertyChanged += OnArtifactPropertyChanged;
        }

        /// <summary>
        /// The current workspace
        /// </summary>
        public WorkspaceArtifact? Workspace
        {
            get => _workspace;
            private set
            {
                if (SetProperty(ref _workspace, value))
                {
                    RootArtifact = value;
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
        /// Get context menu commands for an artifact
        /// </summary>
        public override IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact)
        {
            return TreeViewController.GetContextMenuCommands(artifact);
        }

        /// <summary>
        /// Handle artifact double-click
        /// </summary>
        public override async Task HandleDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default)
        {
            await TreeViewController.HandleDoubleClickAsync(artifact, cancellationToken);
        }

        /// <summary>
        /// Handle artifact rename from the tree view
        /// </summary>
        //public override void OnArtifactRenamed(IArtifact artifact, string oldName, string newName)
        //{
        //    if (oldName != newName)
        //    {
        //        IsDirty = true;
        //        _workspaceController.OnArtifactRenamed(artifact, oldName, newName);
        //    }
        //}

        //public override void SetArtifactName(IArtifact artifact, string newName)
        //{
        //    base.SetArtifactName(artifact, newName);
        //}

        private void OnWorkspaceChanged(object? sender, WorkspaceArtifact? workspace)
        {
            Workspace = workspace;
            SelectedArtifact = workspace;
            IsDirty = false;
        }

        //private void OnBeginRenameRequested(object? sender, IArtifact artifact)
        //{
        //    RequestBeginRename(artifact);
        //}

        //private void OnArtifactPropertyChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        //{
        //    IsDirty = true;
        //    RequestArtifactRefresh(e.Artifact);
        //}

        public override void Cleanup()
        {
            TreeViewController.WorkspaceChanged -= OnWorkspaceChanged;
           // _workspaceController.BeginRenameRequested -= OnBeginRenameRequested;
            //_workspaceController.ArtifactPropertyChanged -= OnArtifactPropertyChanged;
        }
    }
}
