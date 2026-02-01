using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntityEditViewArtifact
    /// </summary>
    public class EntityEditViewArtifactController : ArtifactControllerBase<WorkspaceTreeViewController, EntityEditViewArtifact>
    {
        private EntityEditViewEditViewModel? _editViewModel;

        public EntityEditViewArtifactController(
            WorkspaceTreeViewController workspaceController,
            ILogger<EntityEditViewArtifactController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntityEditViewArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntityEditViewArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Field command with property selection
            var addFieldCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_field",
                Text = "Add Field",
                IconKey = "plus",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            var entityState = artifact.EntityState;
            if (entityState != null)
            {
                foreach (var property in entityState.Properties)
                {
                    var propertyName = property.Name;
                    // Check if field already exists for this property
                    var existingField = artifact.GetFields().FirstOrDefault(f => f.PropertyName == propertyName);
                    if (existingField == null)
                    {
                        addFieldCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                        {
                            Id = $"add_field_{propertyName}",
                            Text = propertyName,
                            IconKey = "text-cursor-input",
                            Execute = async (a) =>
                            {
                                var field = new EntityEditViewFieldArtifact(propertyName)
                                {
                                    ControlType = DataFieldControlType.SingleLineTextField,
                                    DisplayOrder = artifact.GetFields().Count()
                                };
                                artifact.AddField(field);
                                TreeViewController.OnArtifactAdded(artifact, field);
                                await Task.CompletedTask;
                            }
                        });
                    }
                }
            }

            if (addFieldCommand.SubCommands.Count == 0)
            {
                addFieldCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = "no_properties",
                    Text = "(No available properties)",
                    IconKey = "alert-circle"
                });
            }

            commands.Add(addFieldCommand);

            // Add all fields command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_all_fields",
                Text = "Add All Fields",
                IconKey = "layers",
                Execute = async (a) =>
                {
                    var state = artifact.EntityState;
                    if (state != null)
                    {
                        int order = artifact.GetFields().Count();
                        foreach (var property in state.Properties)
                        {
                            var existingField = artifact.GetFields().FirstOrDefault(f => f.PropertyName == property.Name);
                            if (existingField == null)
                            {
                                var field = new EntityEditViewFieldArtifact(property.Name)
                                {
                                    ControlType = DataFieldControlType.SingleLineTextField,
                                    DisplayOrder = order++
                                };
                                artifact.AddField(field);
                                TreeViewController.OnArtifactAdded(artifact, field);
                            }
                        }
                    }
                    await Task.CompletedTask;
                }
            });

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_editview",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            // Properties command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "editview_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(EntityEditViewArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(EntityEditViewArtifact artifact)
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

        protected override Task OnSelectedInternalAsync(EntityEditViewArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(EntityEditViewArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new EntityEditViewEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.EditView = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(EntityEditViewArtifact editView)
        {
            EnsureEditViewModel(editView);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
