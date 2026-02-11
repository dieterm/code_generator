using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class OperatorElementArtifactController : CodeElementArtifactControllerBase<OperatorElementArtifact>
{
    private OperatorElementEditViewModel? _editViewModel;

    public OperatorElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<OperatorElementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger)
    {
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(OperatorElementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "operator_properties",
            Text = "Properties",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "delete_operator",
            Text = "Delete operator",
            Execute = async (a) =>
            {
                var parent = artifact.Parent as OperatorsContainerArtifact;
                parent?.RemoveOperatorElement(artifact);
            }
        };
    }

    protected override Task OnSelectedInternalAsync(OperatorElementArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private Task ShowPropertiesAsync(OperatorElementArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new OperatorElementEditViewModel();
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
