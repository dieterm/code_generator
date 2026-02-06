using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Shared.UndoRedo;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Entities
{
    /// <summary>
    /// Controller for EntitiesContainerArtifact
    /// </summary>
    public class EntitiesContainerController : WorkspaceArtifactControllerBase<EntitiesContainerArtifact>
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
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
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

                    //TreeViewController.UndoRedoManager.RecordAction(new ChildChangeAction(
                    //    artifact,
                    //    entity,
                    //    ChildChangeType.Added,
                    //    addChild: (parent, child) =>
                    //    {
                    //        var container = (EntitiesContainerArtifact)parent;
                    //        var entityToAdd = (EntityArtifact)child;
                    //        container.AddEntity(entityToAdd);
                    //        TreeViewController.OnArtifactAdded(container, entityToAdd);
                    //    },
                    //    removeChild: (parent, child) =>
                    //    {
                    //        var container = (EntitiesContainerArtifact)parent;
                    //        var entityToRemove = (EntityArtifact)child;
                    //        container.RemoveEntity(entityToRemove);
                    //        TreeViewController.OnArtifactRemoved(container, entityToRemove);
                    //    }
                    //));

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
