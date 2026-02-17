using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers.CodeElements;

public class GenericTypeParametersContainerArtifactController : CodeElementArtifactControllerBase<GenericTypeParametersContainerArtifact>
{
    public GenericTypeParametersContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<GenericTypeParametersContainerArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(GenericTypeParametersContainerArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        { Id = "add_generic_type_parameter", Text = "Add Generic Type Parameter", Execute = async (a) => artifact.AddNewParameter() };
    }
}
