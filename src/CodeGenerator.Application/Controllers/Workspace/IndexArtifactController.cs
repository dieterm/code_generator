using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.ViewModels.Datasources;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for IndexArtifact
    /// </summary>
    public class IndexArtifactController : WorkspaceArtifactControllerBase<IndexArtifact, IndexArtifactEditViewModel>
    {
        public IndexArtifactController(OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<IndexArtifactController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(IndexArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_index",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            // Note: Delete command is now added automatically by base class via GetClipboardCommands()

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(IndexArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(IndexArtifact artifact)
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
