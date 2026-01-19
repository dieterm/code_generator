using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Excel.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Excel.ViewModels;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Datasources;

/// <summary>
/// Controller for Excel datasource artifacts
/// </summary>
public class ExcelDatasourceController : ArtifactControllerBase<WorkspaceTreeViewController, ExcelDatasourceArtifact>
{
    private ExcelDatasourceEditViewModel? _editViewModel;

    public ExcelDatasourceController(
        WorkspaceTreeViewController workspaceController,
        ILogger<ExcelDatasourceController> logger)
        : base(workspaceController, logger)
    {
    }

    /// <summary>
    /// Handle Treeview EditLabel complete event
    /// </summary>
    protected override void OnArtifactRenamedInternal(ExcelDatasourceArtifact artifact, string oldName, string newName)
    {
        artifact.Name = newName;
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ExcelDatasourceArtifact artifact)
    {
        var commands = new List<ArtifactTreeNodeCommand>();

        // Rename command
        commands.Add(new ArtifactTreeNodeCommand
        {
            Id = "rename_datasource",
            Text = "Rename",
            IconKey = "edit",
            Execute = async (a) =>
            {
                TreeViewController.RequestBeginRename(artifact);
                await Task.CompletedTask;
            }
        });

        commands.Add(ArtifactTreeNodeCommand.Separator);

        // Refresh sheets command
        commands.Add(new ArtifactTreeNodeCommand
        {
            Id = "refresh_sheets",
            Text = "Refresh Sheets",
            IconKey = "refresh-cw",
            Execute = async (a) =>
            {
                // Re-show the edit view to refresh
                await ShowPropertiesAsync(artifact);
            }
        });

        commands.Add(ArtifactTreeNodeCommand.Separator);

        // Delete command
        commands.Add(new ArtifactTreeNodeCommand
        {
            Id = "delete_datasource",
            Text = "Delete",
            IconKey = "trash",
            Execute = async (a) =>
            {
                var parent = artifact.Parent;
                if (parent != null)
                {
                    parent.RemoveChild(artifact);
                    TreeViewController.OnArtifactRemoved(parent, artifact);
                }
                await Task.CompletedTask;
            }
        });

        // Properties command
        commands.Add(new ArtifactTreeNodeCommand
        {
            Id = "datasource_properties",
            Text = "Properties",
            IconKey = "settings",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        });

        return commands;
    }

    protected override Task OnSelectedInternalAsync(ExcelDatasourceArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private void EnsureEditViewModel(ExcelDatasourceArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new ExcelDatasourceEditViewModel();
            _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            _editViewModel.AddSheetRequested += OnAddSheetRequested;
        }

        _editViewModel.Datasource = artifact;
    }

    private void OnEditViewModelValueChanged(object? sender, PropertyValueChangedEventArgs e)
    {
        if (_editViewModel?.Datasource is IArtifact artifact)
        {
            TreeViewController.OnArtifactPropertyChanged(artifact, e.PropertyName, e.Value);
        }
    }

    private void OnAddSheetRequested(object? sender, AddSheetEventArgs e)
    {
        if (_editViewModel?.Datasource == null) return;

        var datasource = _editViewModel.Datasource;

        if (e.Sheet is TableArtifact table)
        {
            datasource.AddChild(table);
            TreeViewController.OnArtifactAdded(datasource, table);
            Logger.LogInformation("Added sheet {SheetName} to datasource {DatasourceName}",
                table.Name, datasource.Name);
        }
    }

    private Task ShowPropertiesAsync(ExcelDatasourceArtifact datasource)
    {
        EnsureEditViewModel(datasource);
        TreeViewController.ShowArtifactDetailsView(_editViewModel!);
        return Task.CompletedTask;
    }
}
