using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class OperatorsContainerArtifactController : CodeElementArtifactControllerBase<OperatorsContainerArtifact>
{
    public OperatorsContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<OperatorsContainerArtifactController> logger)
        : base(operationExecutor, treeViewController, logger)
    {
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(OperatorsContainerArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "AddOperator",
            Text = "Add Operator",
            IconKey = "AddOperatorIcon",
            Execute = async (a) =>
            {
                var newOperatorElement = new OperatorElement(OperatorType.Addition, new TypeReference("object"));
                artifact.AddOperatorElement(newOperatorElement);
            }
        };
    }
}
