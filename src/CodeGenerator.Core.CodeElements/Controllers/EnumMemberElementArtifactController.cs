using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class EnumMemberElementArtifactController : CodeElementArtifactControllerBase<EnumMemberElementArtifact>
{
    public EnumMemberElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<EnumMemberElementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EnumMemberElementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
        { Id = "rename_enum_member", Text = "Rename", Execute = async (a) => TreeViewController.RequestBeginRename(artifact) };
    }
}
