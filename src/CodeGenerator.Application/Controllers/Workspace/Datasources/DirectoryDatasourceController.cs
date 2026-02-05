using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Directory.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Directory.ViewModels;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Datasources;

/// <summary>
/// Controller for Directory datasource artifacts
/// </summary>
public class DirectoryDatasourceController : WorkspaceArtifactControllerBase<DirectoryDatasourceArtifact>
{
    private DirectoryDatasourceEditViewModel? _editViewModel;

    public DirectoryDatasourceController(
        WorkspaceTreeViewController workspaceController,
        ILogger<DirectoryDatasourceController> logger)
        : base(workspaceController, logger)
    {
    }

    /// <summary>
    /// Handle Treeview EditLabel complete event
    /// </summary>
    protected override void OnArtifactRenamedInternal(DirectoryDatasourceArtifact artifact, string oldName, string newName)
    {
        artifact.Name = newName;
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DirectoryDatasourceArtifact artifact)
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

        // Open directory in Explorer command
        if (System.IO.Directory.Exists(artifact.DirectoryPath))
        {
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "open_directory",
                Text = "Open in Explorer",
                IconKey = "folder-open",
                Execute = async (a) =>
                {
                    System.Diagnostics.Process.Start("explorer.exe", artifact.DirectoryPath);
                    await Task.CompletedTask;
                }
            });
        }

        // Refresh command
        commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "refresh_directory",
            Text = "Refresh",
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

        return commands;
    }

    #region Clipboard Operations

    public override bool CanDelete(DirectoryDatasourceArtifact artifact)
    {
        return artifact.Parent != null;
    }

    public override void Delete(DirectoryDatasourceArtifact artifact)
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

    protected override Task OnSelectedInternalAsync(DirectoryDatasourceArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private void EnsureEditViewModel(DirectoryDatasourceArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new DirectoryDatasourceEditViewModel();
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

    private Task ShowPropertiesAsync(DirectoryDatasourceArtifact datasource)
    {
        EnsureEditViewModel(datasource);
        TreeViewController.ShowArtifactDetailsView(_editViewModel!);
        return Task.CompletedTask;
    }
}
