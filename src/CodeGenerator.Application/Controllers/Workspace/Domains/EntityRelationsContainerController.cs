using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntityRelationsContainerArtifact
    /// </summary>
    public class EntityRelationsContainerController : ArtifactControllerBase<WorkspaceTreeViewController, EntityRelationsContainerArtifact>
    {
        public EntityRelationsContainerController(
            WorkspaceTreeViewController workspaceController,
            ILogger<EntityRelationsContainerController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntityRelationsContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntityRelationsContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Relation command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "add_relation",
                Text = "Add Relation",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var relation = new EntityRelationArtifact("NewRelation");
                    artifact.AddRelation(relation);
                    TreeViewController.OnArtifactAdded(artifact, relation);
                    TreeViewController.RequestBeginRename(relation);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(EntityRelationsContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
