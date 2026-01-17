using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Yaml.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Yaml.ViewModels;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace;

/// <summary>
/// Controller for YAML datasource artifacts
/// </summary>
public class YamlDatasourceController : ArtifactControllerBase<WorkspaceTreeViewController, YamlDatasourceArtifact>
{
    private YamlDatasourceEditViewModel? _editViewModel;

    public YamlDatasourceController(
        WorkspaceTreeViewController workspaceController,
        ILogger<YamlDatasourceController> logger)
        : base(workspaceController, logger)
    {
    }

    /// <summary>
    /// Handle Treeview EditLabel complete event
    /// </summary>
    protected override void OnArtifactRenamedInternal(YamlDatasourceArtifact artifact, string oldName, string newName)
    {
        artifact.Name = newName;
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(YamlDatasourceArtifact artifact)
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

        // Refresh file command
        commands.Add(new ArtifactTreeNodeCommand
        {
            Id = "refresh_file",
            Text = "Refresh",
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

    protected override Task OnSelectedInternalAsync(YamlDatasourceArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private void EnsureEditViewModel(YamlDatasourceArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new YamlDatasourceEditViewModel();
            _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            _editViewModel.AddTableRequested += OnAddTableRequested;
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

    private void OnAddTableRequested(object? sender, AddTableEventArgs e)
    {
        if (_editViewModel?.Datasource == null) return;

        var datasource = _editViewModel.Datasource;

        if (e.Table is TableArtifact table)
        {
            datasource.AddChild(table);
            TreeViewController.OnArtifactAdded(datasource, table);
            Logger.LogInformation("Added table {TableName} to datasource {DatasourceName}",
                table.Name, datasource.Name);
        }
    }

    private Task ShowPropertiesAsync(YamlDatasourceArtifact datasource)
    {
        EnsureEditViewModel(datasource);
        TreeViewController.ShowArtifactDetailsView(_editViewModel!);
        return Task.CompletedTask;
    }
}
