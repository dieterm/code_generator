using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class StructElementArtifactController : CodeElementArtifactControllerBase<StructElementArtifact>
{
    private StructElementEditViewModel? _editViewModel;

    public StructElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<StructElementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(StructElementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
        { Id = "rename_struct", Text = "Rename", Execute = async (a) => TreeViewController.RequestBeginRename(artifact) };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        { Id = "struct_properties", Text = "Properties", Execute = async (a) => await ShowPropertiesAsync(artifact) };
    }

    protected override Task OnSelectedInternalAsync(StructElementArtifact artifact, CancellationToken cancellationToken) => ShowPropertiesAsync(artifact);

    private Task ShowPropertiesAsync(StructElementArtifact artifact)
    {
        _editViewModel ??= new StructElementEditViewModel();
        _editViewModel.Artifact = artifact;
        TreeViewController.ShowArtifactDetailsView(_editViewModel);
        return Task.CompletedTask;
    }
}
