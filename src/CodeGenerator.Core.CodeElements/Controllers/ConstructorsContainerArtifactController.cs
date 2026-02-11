using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class ConstructorsContainerArtifactController : CodeElementArtifactControllerBase<ConstructorsContainerArtifact>
{
    public ConstructorsContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<ConstructorsContainerArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ConstructorsContainerArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        { Id = "add_constructor", Text = "Add Constructor", Execute = async (a) => artifact.AddNewConstructor() };
    }
}
