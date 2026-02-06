using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Specifications;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Specifications
{
    /// <summary>
    /// Controller for DomainSpecificationsContainerArtifact
    /// </summary>
    public class DomainSpecificationsContainerController : WorkspaceArtifactControllerBase<DomainSpecificationsContainerArtifact>
    {
        public DomainSpecificationsContainerController(
            WorkspaceTreeViewController workspaceController,
            ILogger<DomainSpecificationsContainerController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(DomainSpecificationsContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainSpecificationsContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Specification command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_specification",
                Text = "Add Specification",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var specification = new DomainSpecificationArtifact("NewSpecification");
                    artifact.AddSpecification(specification);
                    TreeViewController.OnArtifactAdded(artifact, specification);
                    TreeViewController.RequestBeginRename(specification);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(DomainSpecificationsContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
