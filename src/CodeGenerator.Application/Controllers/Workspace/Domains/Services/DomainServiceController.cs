using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Services;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Services
{
    /// <summary>
    /// Controller for DomainServiceArtifact
    /// </summary>
    public class DomainServiceController : WorkspaceArtifactControllerBase<DomainServiceArtifact>
    {
        public DomainServiceController(
            OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainServiceController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(DomainServiceArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainServiceArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Method command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_service_method",
                Text = "Add Method",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var method = new DomainServiceMethodArtifact("NewMethod");
                    artifact.AddMethod(method);
                    TreeViewController.OnArtifactAdded(artifact, method);
                    TreeViewController.RequestBeginRename(method);
                    await Task.CompletedTask;
                }
            });

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_domain_service",
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

        public override bool CanDelete(DomainServiceArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(DomainServiceArtifact artifact)
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

        protected override Task OnSelectedInternalAsync(DomainServiceArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view yet for domain services
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
