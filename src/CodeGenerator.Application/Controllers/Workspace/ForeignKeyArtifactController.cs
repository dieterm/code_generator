using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for ForeignKeyArtifact
    /// </summary>
    public class ForeignKeyArtifactController : ArtifactControllerBase<WorkspaceTreeViewController, ForeignKeyArtifact>
    {
        private ForeignKeyEditViewModel? _editViewModel;

        public ForeignKeyArtifactController(
            WorkspaceTreeViewController workspaceController,
            ILogger<ForeignKeyArtifactController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ForeignKeyArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "rename_foreignkey",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            // Note: Delete command is now added automatically by base class via GetClipboardCommands()

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(ForeignKeyArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(ForeignKeyArtifact artifact)
        {
            if (!CanDelete(artifact)) return;

            var parent = artifact.Parent;
            if (parent != null)
            {
                parent.RemoveChild(artifact);
                TreeViewController.OnArtifactRemoved(parent, artifact);
            }
        }

        #endregion

        protected override Task OnSelectedInternalAsync(ForeignKeyArtifact artifact, CancellationToken cancellationToken)
        {
            EnsureEditViewModel(artifact);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }

        private void EnsureEditViewModel(ForeignKeyArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new ForeignKeyEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.ForeignKey = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }
    }
}
