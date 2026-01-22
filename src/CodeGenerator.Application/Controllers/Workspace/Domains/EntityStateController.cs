using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Domain.DataTypes;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntityStateArtifact
    /// </summary>
    public class EntityStateController : ArtifactControllerBase<WorkspaceTreeViewController, EntityStateArtifact>
    {
        private EntityStateEditViewModel? _editViewModel;

        public EntityStateController(
            WorkspaceTreeViewController workspaceController,
            ILogger<EntityStateController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntityStateArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntityStateArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Property command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "add_property",
                Text = "Add Property",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var property = new PropertyArtifact("NewProperty", GenericDataTypes.VarChar.Id, true);
                    artifact.AddProperty(property);
                    TreeViewController.OnArtifactAdded(artifact, property);
                    TreeViewController.RequestBeginRename(property);
                    await Task.CompletedTask;
                }
            });

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "rename_state",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            // Delete command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "delete_state",
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
                Id = "state_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(EntityStateArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(EntityStateArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new EntityStateEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.EntityState = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(EntityStateArtifact state)
        {
            EnsureEditViewModel(state);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
