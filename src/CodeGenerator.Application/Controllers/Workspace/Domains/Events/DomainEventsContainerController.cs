using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Events;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Events
{
    /// <summary>
    /// Controller for DomainEventsContainerArtifact
    /// </summary>
    public class DomainEventsContainerController : WorkspaceArtifactControllerBase<DomainEventsContainerArtifact>
    {
        public DomainEventsContainerController(
            OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainEventsContainerController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(DomainEventsContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainEventsContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Domain Event command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_domain_event",
                Text = "Add Domain Event",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var domainEvent = new DomainEventArtifact("NewDomainEvent");
                    artifact.AddDomainEvent(domainEvent);
                    TreeViewController.OnArtifactAdded(artifact, domainEvent);
                    TreeViewController.RequestBeginRename(domainEvent);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(DomainEventsContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
