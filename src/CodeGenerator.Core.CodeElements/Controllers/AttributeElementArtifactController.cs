using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class AttributeElementArtifactController : CodeElementArtifactControllerBase<AttributeElementArtifact>
{
    private AttributeElementEditViewModel? _editViewModel;

    public AttributeElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<AttributeElementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(AttributeElementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
        {
            Id = "rename_attribute", Text = "Rename",
            Execute = async (a) => TreeViewController.RequestBeginRename(artifact)
        };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "attribute_properties", Text = "Properties",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_DELETE)
        {
            Id = "delete_attribute", Text = "Delete",
            Execute = async (a) => await DeleteAttributeAsync(artifact)
        };
    }

    private Task DeleteAttributeAsync(AttributeElementArtifact artifact)
    {
        artifact.Parent?.RemoveChild(artifact);
        return Task.CompletedTask;
    }

    protected override Task OnSelectedInternalAsync(AttributeElementArtifact artifact, CancellationToken cancellationToken) => ShowPropertiesAsync(artifact);

    private Task ShowPropertiesAsync(AttributeElementArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new AttributeElementEditViewModel();
            _editViewModel.ValueChanged += OnEditViewModelValueChanged;
        }

        _editViewModel.Artifact = artifact;
        TreeViewController.ShowArtifactDetailsView(_editViewModel);
        return Task.CompletedTask;
    }

    private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
    {
        TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
    }
}
