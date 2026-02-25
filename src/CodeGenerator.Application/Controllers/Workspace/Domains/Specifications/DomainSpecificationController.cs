using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Specifications;
using CodeGenerator.Core.Workspaces.ViewModels.Domain;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Specifications
{
    /// <summary>
    /// Controller for DomainSpecificationArtifact
    /// </summary>
    public class DomainSpecificationController : WorkspaceArtifactControllerBase<DomainSpecificationArtifact, DomainSpecificationArtifactEditViewModel>
    {
        public DomainSpecificationController(OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainSpecificationController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(DomainSpecificationArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainSpecificationArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_specification",
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
                Id = "specification_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(DomainSpecificationArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(DomainSpecificationArtifact artifact)
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
