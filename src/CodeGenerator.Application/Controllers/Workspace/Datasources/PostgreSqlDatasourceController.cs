using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.ViewModels;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Datasources;

/// <summary>
/// Controller for PostgreSQL datasource artifacts
/// </summary>
public class PostgreSqlDatasourceController : ArtifactControllerBase<WorkspaceTreeViewController, PostgreSqlDatasourceArtifact>
{
    private PostgreSqlDatasourceEditViewModel? _editViewModel;

    public PostgreSqlDatasourceController(
        WorkspaceTreeViewController workspaceController,
        ILogger<PostgreSqlDatasourceController> logger)
        : base(workspaceController, logger)
    {
    }

    /// <summary>
    /// Handle Treeview EditLabel complete event
    /// </summary>
    protected override void OnArtifactRenamedInternal(PostgreSqlDatasourceArtifact artifact, string oldName, string newName)
    {
        artifact.Name = newName;
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(PostgreSqlDatasourceArtifact artifact)
    {
        var commands = new List<ArtifactTreeNodeCommand>();

        // Rename command
        commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
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

        // Add table command
        commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "add_table",
            Text = "Add Table",
            IconKey = "table",
            Execute = async (a) =>
            {
                var table = new TableArtifact("new_table", "public");
                artifact.AddChild(table);
                TreeViewController.OnArtifactAdded(artifact, table);
                await Task.CompletedTask;
            }
        });

        // Add view command
        commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "add_view",
            Text = "Add View",
            IconKey = "eye",
            Execute = async (a) =>
            {
                var view = new ViewArtifact("new_view", "public");
                artifact.AddChild(view);
                TreeViewController.OnArtifactAdded(artifact, view);
                await Task.CompletedTask;
            }
        });

        // Refresh schema command
        commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "refresh_schema",
            Text = "Refresh Schema",
            IconKey = "refresh-cw",
            Execute = async (a) =>
            {
                // Re-show the edit view to refresh
                await ShowPropertiesAsync(artifact);
            }
        });

        // Properties command
        commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "datasource_properties",
            Text = "Properties",
            IconKey = "settings",
            Execute = async (a) => await ShowPropertiesAsync(artifact)
        });

        // Note: Delete command is now added automatically by base class via GetClipboardCommands()

        return commands;
    }

    #region Clipboard Operations

    public override bool CanDelete(PostgreSqlDatasourceArtifact artifact)
    {
        return artifact.Parent != null;
    }

    public override void Delete(PostgreSqlDatasourceArtifact artifact)
    {
        if (!CanDelete(artifact)) return;

        var parent = artifact.Parent;
        if (parent != null)
        {
            parent.RemoveChild(artifact);
            TreeViewController.OnArtifactRemoved(parent, artifact);
        }
    }

    #endregion

    protected override Task OnSelectedInternalAsync(PostgreSqlDatasourceArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private void EnsureEditViewModel(PostgreSqlDatasourceArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new PostgreSqlDatasourceEditViewModel();
            _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            _editViewModel.AddObjectRequested += OnAddObjectRequested;
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

    private void OnAddObjectRequested(object? sender, AddDatabaseObjectEventArgs e)
    {
        if (_editViewModel?.Datasource == null) return;

        var datasource = _editViewModel.Datasource;

        if (e.DatabaseObject is TableArtifact table)
        {
            datasource.AddChild(table);
            TreeViewController.OnArtifactAdded(datasource, table);
            datasource.TryCompleteForeignKeys(table, Logger);
            Logger.LogInformation("Added table {TableName} to datasource {DatasourceName}",
                table.Name, datasource.Name);
        }
        else if (e.DatabaseObject is ViewArtifact view)
        {
            datasource.AddChild(view);
            TreeViewController.OnArtifactAdded(datasource, view);
            Logger.LogInformation("Added view {ViewName} to datasource {DatasourceName}",
                view.Name, datasource.Name);
        }
    }

    private Task ShowPropertiesAsync(PostgreSqlDatasourceArtifact datasource)
    {
        EnsureEditViewModel(datasource);
        TreeViewController.ShowArtifactDetailsView(_editViewModel!);
        return Task.CompletedTask;
    }
}
