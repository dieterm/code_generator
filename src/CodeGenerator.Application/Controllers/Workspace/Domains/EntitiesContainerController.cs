using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntitiesContainerArtifact
    /// </summary>
    public class EntitiesContainerController : ArtifactControllerBase<WorkspaceTreeViewController, EntitiesContainerArtifact>
    {
        public EntitiesContainerController(
            WorkspaceTreeViewController workspaceController,
            ILogger<EntitiesContainerController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntitiesContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntitiesContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Entity command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "add_entity",
                Text = "Add Entity",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var entity = new EntityArtifact("NewEntity");
                    artifact.AddEntity(entity);
                    TreeViewController.OnArtifactAdded(artifact, entity);
                    TreeViewController.RequestBeginRename(entity);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(EntitiesContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
