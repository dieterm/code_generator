using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers;

public class EventElementArtifactController : CodeElementArtifactControllerBase<EventElementArtifact>
{
    private EventElementEditViewModel? _editViewModel;

    public EventElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<EventElementArtifactController> logger)
        : base(operationExecutor, treeViewController, logger) { }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EventElementArtifact artifact)
    {
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
        { Id = "rename_event", Text = "Rename", Execute = async (a) => TreeViewController.RequestBeginRename(artifact) };
        yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        { Id = "event_properties", Text = "Properties", Execute = async (a) => await ShowPropertiesAsync(artifact) };
    }

    protected override Task OnSelectedInternalAsync(EventElementArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private Task ShowPropertiesAsync(EventElementArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new EventElementEditViewModel();
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
