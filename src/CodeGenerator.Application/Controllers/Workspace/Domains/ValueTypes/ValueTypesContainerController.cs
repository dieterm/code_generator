using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.ValueTypes
{
    /// <summary>
    /// Controller for ValueTypesContainerArtifact
    /// </summary>
    public class ValueTypesContainerController : WorkspaceArtifactControllerBase<ValueTypesContainerArtifact>
    {
        public ValueTypesContainerController(
            WorkspaceTreeViewController workspaceController,
            ILogger<ValueTypesContainerController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(ValueTypesContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ValueTypesContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add ValueType command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_valuetype",
                Text = "Add Value Type",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var valueType = new ValueTypeArtifact("NewValueType");
                    artifact.AddValueType(valueType);
                    TreeViewController.OnArtifactAdded(artifact, valueType);
                    TreeViewController.RequestBeginRename(valueType);
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(ValueTypesContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
