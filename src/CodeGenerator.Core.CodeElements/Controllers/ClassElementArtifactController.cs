using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class ClassElementArtifactController : CodeElementArtifactControllerBase<ClassElementArtifact>
{
    private ClassElementEditViewModel? _editViewModel;

    public ClassElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<ClassElementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ClassElementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
        {
            Id = "rename_class", Text = "Rename",
            Execute = async (a) => TreeViewController.RequestBeginRename(artifact)
        };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "class_properties", Text = "Properties",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        };
    }

    protected override Task OnSelectedInternalAsync(ClassElementArtifact artifact, CancellationToken cancellationToken) => ShowPropertiesAsync(artifact);

    private Task ShowPropertiesAsync(ClassElementArtifact artifact)
    {
        _editViewModel ??= new ClassElementEditViewModel();
        _editViewModel.Artifact = artifact;
        TreeViewController.ShowArtifactDetailsView(_editViewModel);
        return Task.CompletedTask;
    }
}
