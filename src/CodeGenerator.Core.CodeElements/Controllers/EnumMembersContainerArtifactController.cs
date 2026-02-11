using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class EnumMembersContainerArtifactController : CodeElementArtifactControllerBase<EnumMembersContainerArtifact>
{
    public EnumMembersContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<EnumMembersContainerArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EnumMembersContainerArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "add_enum_member", Text = "Add Member",
            Execute = async (a) => artifact.AddNewMember()
        };
    }
}
