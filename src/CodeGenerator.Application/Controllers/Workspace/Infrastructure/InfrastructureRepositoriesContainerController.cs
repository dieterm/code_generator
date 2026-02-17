using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for InfrastructureRepositoriesContainer
    /// </summary>
    public class InfrastructureRepositoriesContainerController : WorkspaceArtifactControllerBase<InfrastructureRepositoriesContainerArtifact>
    {
        public InfrastructureRepositoriesContainerController(
            OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<InfrastructureRepositoriesContainerController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(InfrastructureRepositoriesContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(InfrastructureRepositoriesContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Repository command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_repository",
                Text = "Add Repository",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var childItem = new RepositoryImplementationArtifact("New repository");
                    artifact.AddChild(childItem);
                    TreeViewController.OnArtifactAdded(artifact, childItem);
                    TreeViewController.RequestBeginRename(childItem);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(InfrastructureRepositoriesContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
