using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers.Statements;

public class ThrowStatementArtifactController : StatementArtifactControllerBase<ThrowStatementArtifact>
{
    private ThrowStatementEditViewModel? _editViewModel;

    public ThrowStatementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<ThrowStatementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger)
    {
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ThrowStatementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "properties",
            Text = "Properties",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        };
    }

    public override bool CanDelete(ThrowStatementArtifact artifact)
    {
        return artifact.Parent != null;
    }

    public override void Delete(ThrowStatementArtifact artifact)
    {
        if (artifact.Parent is CompositeStatementArtifact parent)
        {
            parent.Statements.Remove(artifact.StatementElement);
            parent.RemoveChild(artifact);
        }
        else
        {
            throw new NotImplementedException("Deletion of ThrowStatementArtifact is only implemented for those with a CompositeStatementArtifact parent.");
        }
    }

    protected override async Task OnSelectedInternalAsync(ThrowStatementArtifact artifact, CancellationToken cancellationToken)
    {
        await ShowPropertiesAsync(artifact);
    }

    private Task ShowPropertiesAsync(ThrowStatementArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new ThrowStatementEditViewModel();
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
