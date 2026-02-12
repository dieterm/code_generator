using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers.Statements;

public class RawStatementArtifactController : StatementArtifactControllerBase<RawStatementArtifact>
{
    private RawStatementEditViewModel? _editViewModel;

    public RawStatementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<RawStatementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger)
    {
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(RawStatementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "properties",
            Text = "Properties",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        };
    }

    public override bool CanDelete(RawStatementArtifact artifact)
    {
        return artifact.Parent != null;
    }

    public override void Delete(RawStatementArtifact artifact)
    {
        if (artifact.Parent is CompositeStatementArtifact parent)
        {
            parent.Statements.Remove(artifact.StatementElement);
            parent.RemoveChild(artifact);
        }
        else
        {
            throw new NotImplementedException("Deletion of RawStatementArtifact is only implemented for those with a CompositeStatementArtifact parent.");
        }
    }

    protected override async Task OnSelectedInternalAsync(RawStatementArtifact artifact, CancellationToken cancellationToken)
    {
        await ShowPropertiesAsync(artifact);
    }

    private Task ShowPropertiesAsync(RawStatementArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new RawStatementEditViewModel();
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
