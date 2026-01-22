using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntityArtifact
    /// </summary>
    public class EntityController : ArtifactControllerBase<WorkspaceTreeViewController, EntityArtifact>
    {
        private EntityEditViewModel? _editViewModel;

        public EntityController(
            WorkspaceTreeViewController workspaceController,
            ILogger<EntityController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntityArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntityArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "rename_entity",
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
                Id = "delete_entity",
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

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Properties command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "entity_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(EntityArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(EntityArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new EntityEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Entity = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(EntityArtifact entity)
        {
            EnsureEditViewModel(entity);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
