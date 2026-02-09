using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Repositories;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Repositories
{
    /// <summary>
    /// Controller for DomainRepositoriesContainerArtifact
    /// </summary>
    public class DomainRepositoriesContainerController : WorkspaceArtifactControllerBase<DomainRepositoriesContainerArtifact>
    {
        public DomainRepositoriesContainerController(
            OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainRepositoriesContainerController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(DomainRepositoriesContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainRepositoriesContainerArtifact artifact)
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
                    var repository = new DomainRepositoryArtifact("INewRepository");
                    artifact.AddRepository(repository);
                    TreeViewController.OnArtifactAdded(artifact, repository);
                    TreeViewController.RequestBeginRename(repository);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(DomainRepositoriesContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
