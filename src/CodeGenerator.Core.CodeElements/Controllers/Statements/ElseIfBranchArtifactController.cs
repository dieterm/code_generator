using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers.Statements;

public class ElseIfBranchArtifactController : StatementArtifactControllerBase<ElseIfBranchArtifact>
{
    private ElseIfBranchEditViewModel? _editViewModel;

    public ElseIfBranchArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<ElseIfBranchArtifactController> logger)
        : base(operationExecutor, treeViewController, logger)
    {
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ElseIfBranchArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "properties",
            Text = "Properties",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        };
    }

    public override bool CanDelete(ElseIfBranchArtifact artifact)
    {
        return artifact.Parent is IfStatementArtifact;
    }

    public override void Delete(ElseIfBranchArtifact artifact)
    {
        if (artifact.Parent is IfStatementArtifact parent)
        {
            parent.ElseIfBranches.Remove(artifact.StatementElement);
            parent.RemoveChild(artifact);
        }
    }

    protected override async Task OnSelectedInternalAsync(ElseIfBranchArtifact artifact, CancellationToken cancellationToken)
    {
        await ShowPropertiesAsync(artifact);
    }

    private Task ShowPropertiesAsync(ElseIfBranchArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new ElseIfBranchEditViewModel();
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
