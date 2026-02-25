using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.ViewModels;
using CodeGenerator.Core.Workspaces.ViewModels.Domain;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for DomainArtifact
    /// </summary>
    public class DomainArtifactController : WorkspaceArtifactControllerBase<DomainArtifact, DomainArtifactEditViewModel>
    {
        public DomainArtifactController(
            OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainArtifactController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
        }

        /// <summary>
        /// Handle Treeview EditLabel complete event
        /// </summary>
        protected override void OnArtifactRenamedInternal(DomainArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_domain",
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
                Id = "domain_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            // Note: Delete command is now added automatically by base class via GetClipboardCommands()

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(DomainArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(DomainArtifact artifact)
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

    }
}
