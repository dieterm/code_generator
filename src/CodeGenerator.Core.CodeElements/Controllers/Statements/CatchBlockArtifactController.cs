using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers.Statements;

public class CatchBlockArtifactController : StatementArtifactControllerBase<CatchBlockArtifact>
{
    private CatchBlockEditViewModel? _editViewModel;

    public CatchBlockArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<CatchBlockArtifactController> logger)
        : base(operationExecutor, treeViewController, logger)
    {
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(CatchBlockArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "properties",
            Text = "Properties",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        };
    }

    public override bool CanDelete(CatchBlockArtifact artifact)
    {
        return artifact.Parent is TryCatchStatementArtifact;
    }

    public override void Delete(CatchBlockArtifact artifact)
    {
        if (artifact.Parent is TryCatchStatementArtifact parent)
        {
            parent.CatchBlocks.Remove(artifact.StatementElement);
            parent.RemoveChild(artifact);
        }
    }

    protected override async Task OnSelectedInternalAsync(CatchBlockArtifact artifact, CancellationToken cancellationToken)
    {
        await ShowPropertiesAsync(artifact);
    }

    private Task ShowPropertiesAsync(CatchBlockArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new CatchBlockEditViewModel();
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
