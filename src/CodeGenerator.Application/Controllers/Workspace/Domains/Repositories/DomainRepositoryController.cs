using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Repositories;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Repositories
{
    /// <summary>
    /// Controller for DomainRepositoryArtifact
    /// </summary>
    public class DomainRepositoryController : WorkspaceArtifactControllerBase<DomainRepositoryArtifact>
    {
        public DomainRepositoryController(
            OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainRepositoryController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(DomainRepositoryArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainRepositoryArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Method command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_repository_method",
                Text = "Add Method",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var method = new DomainRepositoryMethodArtifact("NewMethod");
                    artifact.AddMethod(method);
                    TreeViewController.OnArtifactAdded(artifact, method);
                    TreeViewController.RequestBeginRename(method);
                    await Task.CompletedTask;
                }
            });

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_repository",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });


            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(DomainRepositoryArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(DomainRepositoryArtifact artifact)
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

        protected override Task OnSelectedInternalAsync(DomainRepositoryArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view yet for repositories
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
