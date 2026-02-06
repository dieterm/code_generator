using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Services;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Services
{
    /// <summary>
    /// Controller for DomainServiceMethodArtifact
    /// </summary>
    public class DomainServiceMethodController : WorkspaceArtifactControllerBase<DomainServiceMethodArtifact>
    {
        public DomainServiceMethodController(
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainServiceMethodController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(DomainServiceMethodArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainServiceMethodArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_service_method",
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

        public override bool CanDelete(DomainServiceMethodArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(DomainServiceMethodArtifact artifact)
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

        protected override Task OnSelectedInternalAsync(DomainServiceMethodArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view yet for service methods
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
