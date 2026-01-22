using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.ViewModels;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntityRelationArtifact
    /// </summary>
    public class EntityRelationController : ArtifactControllerBase<WorkspaceTreeViewController, EntityRelationArtifact>
    {
        private EntityRelationEditViewModel? _editViewModel;

        public EntityRelationController(
            WorkspaceTreeViewController workspaceController,
            ILogger<EntityRelationController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntityRelationArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntityRelationArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "rename_relation",
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
                Id = "delete_relation",
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

        protected override Task OnSelectedInternalAsync(EntityRelationArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(EntityRelationArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new EntityRelationEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Relation = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(EntityRelationArtifact relation)
        {
            EnsureEditViewModel(relation);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
