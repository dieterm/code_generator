using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class InterfaceElementArtifactController : CodeElementArtifactControllerBase<InterfaceElementArtifact>
{
    private InterfaceElementEditViewModel? _editViewModel;

    public InterfaceElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<InterfaceElementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(InterfaceElementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
        { Id = "rename_interface", Text = "Rename", Execute = async (a) => TreeViewController.RequestBeginRename(artifact) };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        { Id = "interface_properties", Text = "Properties", Execute = async (a) => await ShowPropertiesAsync(artifact) };
    }

    protected override Task OnSelectedInternalAsync(InterfaceElementArtifact artifact, CancellationToken cancellationToken) => ShowPropertiesAsync(artifact);

    private Task ShowPropertiesAsync(InterfaceElementArtifact artifact)
    {
        _editViewModel ??= new InterfaceElementEditViewModel();
        _editViewModel.Artifact = artifact;
        TreeViewController.ShowArtifactDetailsView(_editViewModel);
        return Task.CompletedTask;
    }
}
