using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for InfrastructureServicesContainerArtifact
    /// </summary>
    public class InfrastructureServicesContainerArtifactController : WorkspaceArtifactControllerBase<InfrastructureServicesContainerArtifact>
    {
        public InfrastructureServicesContainerArtifactController(
            OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<InfrastructureServicesContainerArtifactController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(InfrastructureServicesContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(InfrastructureServicesContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Service command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_service",
                Text = "Add Service",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var childItem = new ServiceImplementationArtifact("New service");
                    artifact.AddChild(childItem);
                    TreeViewController.OnArtifactAdded(artifact, childItem);
                    TreeViewController.RequestBeginRename(childItem);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(InfrastructureServicesContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
