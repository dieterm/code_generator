using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Factories;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Factories
{
    /// <summary>
    /// Controller for DomainFactoriesContainerArtifact
    /// </summary>
    public class DomainFactoriesContainerController : WorkspaceArtifactControllerBase<DomainFactoriesContainerArtifact>
    {
        public DomainFactoriesContainerController(
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainFactoriesContainerController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(DomainFactoriesContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainFactoriesContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Factory command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_factory",
                Text = "Add Factory",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var factory = new DomainFactoryArtifact("NewFactory");
                    artifact.AddFactory(factory);
                    TreeViewController.OnArtifactAdded(artifact, factory);
                    TreeViewController.RequestBeginRename(factory);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(DomainFactoriesContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
