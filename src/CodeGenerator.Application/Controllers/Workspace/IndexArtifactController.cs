using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for IndexArtifact
    /// </summary>
    public class IndexArtifactController : ArtifactControllerBase<WorkspaceTreeViewController, IndexArtifact>
    {
        private IndexEditViewModel? _editViewModel;

        //protected WorkspaceTreeViewController TreeViewController => (WorkspaceTreeViewController)base.TreeViewController;

        public IndexArtifactController(
            WorkspaceTreeViewController workspaceController,
            ILogger<IndexArtifactController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(IndexArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "rename_index",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Delete command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "delete_index",
                Text = "Delete",
                IconKey = "trash",
                Execute = async (a) =>
                {
                    var parent = artifact.Parent;
                    if (parent != null)
                    {
                        parent.RemoveChild(artifact);
                        TreeViewController.OnArtifactRemoved(parent, artifact);
                    }
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(IndexArtifact artifact, CancellationToken cancellationToken)
        {
            EnsureEditViewModel(artifact);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }

        private void EnsureEditViewModel(IndexArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new IndexEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Index = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }
    }
}
