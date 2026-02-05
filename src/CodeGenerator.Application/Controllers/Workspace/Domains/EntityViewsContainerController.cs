using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntityViewsContainerArtifact
    /// </summary>
    public class EntityViewsContainerController : WorkspaceArtifactControllerBase<EntityViewsContainerArtifact>
    {
        public EntityViewsContainerController(
            WorkspaceTreeViewController workspaceController,
            ILogger<EntityViewsContainerController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntityViewsContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntityViewsContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Get parent entity for state selection
            var entity = artifact.FindAncesterOfType<EntityArtifact>();

            // Add Edit View command with state selection submenu
            var addEditViewCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_editview",
                Text = "Add Edit View",
                IconKey = "pencil",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            if (entity != null)
            {
                foreach (var state in entity.GetStates())
                {
                    var stateId = state.Id;
                    var stateName = state.Name;
                    addEditViewCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                    {
                        Id = $"add_editview_for_{stateId}",
                        Text = $"For State: {stateName}",
                        IconKey = "pencil",
                        Execute = async (a) =>
                        {
                            var editView = new EntityEditViewArtifact($"{stateName}EditView")
                            {
                                EntityStateId = stateId
                            };
                            artifact.AddEditView(editView);
                            TreeViewController.OnArtifactAdded(artifact, editView);
                            TreeViewController.RequestBeginRename(editView);
                            await Task.CompletedTask;
                        }
                    });
                }
            }

            if (addEditViewCommand.SubCommands.Count == 0)
            {
                addEditViewCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = "no_states",
                    Text = "(No states available - add states first)",
                    IconKey = "alert-circle"
                });
            }

            commands.Add(addEditViewCommand);

            // Add List View command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_listview",
                Text = "Add List View",
                IconKey = "table",
                Execute = async (a) =>
                {
                    var listView = new EntityListViewArtifact("DefaultListView");
                    artifact.SetListView(listView);
                    TreeViewController.OnArtifactAdded(artifact, listView);
                    TreeViewController.RequestBeginRename(listView);
                    await Task.CompletedTask;
                }
            });

            // Add Select View command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_selectview",
                Text = "Add Select View",
                IconKey = "chevrons-up-down",
                Execute = async (a) =>
                {
                    var selectView = new EntitySelectViewArtifact("DefaultSelectView");
                    artifact.SetSelectView(selectView);
                    TreeViewController.OnArtifactAdded(artifact, selectView);
                    TreeViewController.RequestBeginRename(selectView);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(EntityViewsContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
