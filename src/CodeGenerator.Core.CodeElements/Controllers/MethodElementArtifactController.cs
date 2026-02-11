using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class MethodElementArtifactController : CodeElementArtifactControllerBase<MethodElementArtifact>
{
    private MethodElementEditViewModel? _editViewModel;

    public MethodElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<MethodElementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(MethodElementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
        { Id = "rename_method", Text = "Rename", Execute = async (a) => TreeViewController.RequestBeginRename(artifact) };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        { Id = "method_properties", Text = "Properties", Execute = async (a) => await ShowPropertiesAsync(artifact) };
    }

    protected override Task OnSelectedInternalAsync(MethodElementArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private Task ShowPropertiesAsync(MethodElementArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new MethodElementEditViewModel();
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
