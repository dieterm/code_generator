using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class NamespaceElementArtifactController : CodeElementArtifactControllerBase<NamespaceElementArtifact>
{
    private NamespaceElementEditViewModel? _editViewModel;

    public NamespaceElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<NamespaceElementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(NamespaceElementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
        {
            Id = "rename_namespace", Text = "Rename",
            Execute = async (a) => TreeViewController.RequestBeginRename(artifact)
        };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "namespace_properties", Text = "Properties",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_DELETE)
        {
            Id = "delete_namespace", Text = "Delete",
            Execute = async (a) => await DeleteNamespaceAsync(artifact)
        };
    }

    private Task DeleteNamespaceAsync(NamespaceElementArtifact artifact)
    {
        var namespacesContainer = (artifact.Parent as NamespacesContainerArtifact)!;
        namespacesContainer.RemoveNamespace(artifact);
        return Task.CompletedTask;
    }

    protected override Task OnSelectedInternalAsync(NamespaceElementArtifact artifact, CancellationToken cancellationToken) => ShowPropertiesAsync(artifact);

    private Task ShowPropertiesAsync(NamespaceElementArtifact artifact)
    {
        _editViewModel ??= new NamespaceElementEditViewModel();
        _editViewModel.Artifact = artifact;
        TreeViewController.ShowArtifactDetailsView(_editViewModel);
        return Task.CompletedTask;
    }
}
