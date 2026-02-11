using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class EnumElementArtifactController : CodeElementArtifactControllerBase<EnumElementArtifact>
{
    private EnumElementEditViewModel? _editViewModel;

    public EnumElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<EnumElementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EnumElementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
        { Id = "rename_enum", Text = "Rename", Execute = async (a) => TreeViewController.RequestBeginRename(artifact) };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        { Id = "enum_properties", Text = "Properties", Execute = async (a) => await ShowPropertiesAsync(artifact) };
    }

    protected override Task OnSelectedInternalAsync(EnumElementArtifact artifact, CancellationToken cancellationToken) => ShowPropertiesAsync(artifact);

    private Task ShowPropertiesAsync(EnumElementArtifact artifact)
    {
        _editViewModel ??= new EnumElementEditViewModel();
        _editViewModel.Artifact = artifact;
        TreeViewController.ShowArtifactDetailsView(_editViewModel);
        return Task.CompletedTask;
    }
}
