using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels.CodeElements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers.CodeElements;

public class TypeReferenceArtifactController : CodeElementArtifactControllerBase<TypeReferenceArtifact>
{
    private TypeReferenceEditViewModel? _editViewModel;

    public TypeReferenceArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<TypeReferenceArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(TypeReferenceArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
        {
            Id = "rename_type_reference", Text = "Rename",
            Execute = async (a) => TreeViewController.RequestBeginRename(artifact)
        };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "type_reference_properties", Text = "Properties",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        };
    }

    public override bool CanDelete(TypeReferenceArtifact artifact)
    {
        return artifact.Parent is BaseTypesContainerArtifact;
    }

    public override void Delete(TypeReferenceArtifact artifact)
    {
        var parentContainer = artifact.Parent as BaseTypesContainerArtifact;
        if (parentContainer == null) return;
        parentContainer.RemoveBaseType(artifact);
    }

    protected override Task OnSelectedInternalAsync(TypeReferenceArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private Task ShowPropertiesAsync(TypeReferenceArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new TypeReferenceEditViewModel();
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
