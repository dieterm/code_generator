using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for IndexArtifact
    /// </summary>
    public class IndexArtifactController : ArtifactControllerBase<IndexArtifact>
    {
        private IndexEditViewModel? _editViewModel;

        public IndexArtifactController(
            WorkspaceController workspaceController,
            ILogger<IndexArtifactController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override IEnumerable<WorkspaceCommand> GetCommands(IndexArtifact artifact)
        {
            var commands = new List<WorkspaceCommand>();

            // Rename command
            commands.Add(new WorkspaceCommand
            {
                Id = "rename_index",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    WorkspaceController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Delete command
            commands.Add(new WorkspaceCommand
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
                        WorkspaceController.OnArtifactRemoved(parent, artifact);
                    }
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(IndexArtifact artifact, CancellationToken cancellationToken)
        {
            EnsureEditViewModel(artifact);
            WorkspaceController.ShowWorkspaceDetailsView(_editViewModel!);
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
            WorkspaceController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }
    }
}
