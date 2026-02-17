using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers.CodeElements;

public class GenericConstraintsContainerArtifactController : CodeElementArtifactControllerBase<GenericConstraintsContainerArtifact>
{
    public GenericConstraintsContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<GenericConstraintsContainerArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(GenericConstraintsContainerArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        { Id = "add_generic_constraint", Text = "Add Generic Constraint", Execute = async (a) => artifact.AddNewConstraint() };
    }
}
