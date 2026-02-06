using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Repositories;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Repositories
{
    /// <summary>
    /// Controller for DomainRepositoryMethodArtifact
    /// </summary>
    public class DomainRepositoryMethodController : WorkspaceArtifactControllerBase<DomainRepositoryMethodArtifact>
    {
        public DomainRepositoryMethodController(
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainRepositoryMethodController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(DomainRepositoryMethodArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainRepositoryMethodArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_repository_method",
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

        public override bool CanDelete(DomainRepositoryMethodArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(DomainRepositoryMethodArtifact artifact)
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

        protected override Task OnSelectedInternalAsync(DomainRepositoryMethodArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view yet for repository methods
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
