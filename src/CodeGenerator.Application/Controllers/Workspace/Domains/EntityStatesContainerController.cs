using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntityStatesContainerArtifact
    /// </summary>
    public class EntityStatesContainerController : WorkspaceArtifactControllerBase<EntityStatesContainerArtifact>
    {
        public EntityStatesContainerController(
            WorkspaceTreeViewController workspaceController,
            ILogger<EntityStatesContainerController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntityStatesContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntityStatesContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add State command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_state",
                Text = "Add State",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var state = new EntityStateArtifact("NewState");
                    artifact.AddState(state);
                    TreeViewController.OnArtifactAdded(artifact, state);
                    TreeViewController.RequestBeginRename(state);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(EntityStatesContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
