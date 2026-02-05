using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntityStateArtifact
    /// </summary>
    public class EntityStateController : WorkspaceArtifactControllerBase<WorkspaceTreeViewController, EntityStateArtifact>
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
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
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

           
            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
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

            // Naming Convention Commands
            var namingConventionCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "properties_naming_convention",
                Text = "Properties Naming Convention",
                IconKey = "edit",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            foreach(var style in Enum.GetValues<NamingStyle>())
            {
                namingConventionCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
                {
                    Id = $"properties_to_{style.ToString().ToLower()}_property",
                    Text = $"To {style}",
                    IconKey = "edit",
                    Execute = async (a) =>
                    {
                        foreach(var property in artifact.Properties)
                        {
                            property.Name = NamingConventions.Convert(property.Name, style);
                        }
                        //artifact.Name = NamingConventions.Convert(artifact.Name, style);
                        await Task.CompletedTask;
                    }
                });
            }
            
            commands.Add(namingConventionCommand);

            
            // Properties command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "state_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            // Note: Delete command is now added automatically by base class via GetClipboardCommands()

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(EntityStateArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(EntityStateArtifact artifact)
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
