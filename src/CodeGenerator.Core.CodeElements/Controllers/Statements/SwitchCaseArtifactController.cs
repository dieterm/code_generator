using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers.Statements;

public class SwitchCaseArtifactController : StatementArtifactControllerBase<SwitchCaseArtifact>
{
    private SwitchCaseEditViewModel? _editViewModel;

    public SwitchCaseArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<SwitchCaseArtifactController> logger)
        : base(operationExecutor, treeViewController, logger)
    {
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(SwitchCaseArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "properties",
            Text = "Properties",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        };
    }

    public override bool CanDelete(SwitchCaseArtifact artifact)
    {
        return artifact.Parent is SwitchStatementArtifact;
    }

    public override void Delete(SwitchCaseArtifact artifact)
    {
        if (artifact.Parent is SwitchStatementArtifact parent)
        {
            parent.Cases.Remove(artifact.StatementElement);
            parent.RemoveChild(artifact);
        }
    }

    protected override async Task OnSelectedInternalAsync(SwitchCaseArtifact artifact, CancellationToken cancellationToken)
    {
        await ShowPropertiesAsync(artifact);
    }

    private Task ShowPropertiesAsync(SwitchCaseArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new SwitchCaseEditViewModel();
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
