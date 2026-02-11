using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class ParametersContainerArtifactController : CodeElementArtifactControllerBase<ParametersContainerArtifact>
{
    public ParametersContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<ParametersContainerArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ParametersContainerArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        { Id = "add_parameter", Text = "Add Parameter", Execute = async (a) => artifact.AddNewParameter() };
    }
}
