using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Services;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Services
{
    /// <summary>
    /// Controller for DomainServicesContainerArtifact
    /// </summary>
    public class DomainServicesContainerController : WorkspaceArtifactControllerBase<DomainServicesContainerArtifact>
    {
        public DomainServicesContainerController(
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainServicesContainerController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(DomainServicesContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainServicesContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Domain Service command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_domain_service",
                Text = "Add Domain Service",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var service = new DomainServiceArtifact("INewDomainService");
                    artifact.AddDomainService(service);
                    TreeViewController.OnArtifactAdded(artifact, service);
                    TreeViewController.RequestBeginRename(service);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(DomainServicesContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
