using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Json.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Json.ViewModels;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Datasources;

/// <summary>
/// Controller for JSON datasource artifacts
/// </summary>
public class JsonDatasourceController : ArtifactControllerBase<WorkspaceTreeViewController, JsonDatasourceArtifact>
{
    private JsonDatasourceEditViewModel? _editViewModel;

    public JsonDatasourceController(
        WorkspaceTreeViewController workspaceController,
        ILogger<JsonDatasourceController> logger)
        : base(workspaceController, logger)
    {
    }

    /// <summary>
    /// Handle Treeview EditLabel complete event
    /// </summary>
    protected override void OnArtifactRenamedInternal(JsonDatasourceArtifact artifact, string oldName, string newName)
    {
        artifact.Name = newName;
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(JsonDatasourceArtifact artifact)
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

        // View/Edit file command
        if (System.IO.File.Exists(artifact.FilePath))
        {
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "edit_datasource",
                Text = "View/Edit file",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    var windowService = ServiceProviderHolder.GetRequiredService<IWindowManagerService>();
                    var previewViewModel = new ViewModels.ArtifactPreviewViewModel
                    {
                        TabLabel = Path.GetFileName(artifact.FilePath),
                        FilePath = artifact.FilePath,
                        TextLanguageSchema = ViewModels.ArtifactPreviewViewModel.KnownLanguages.Text
                    };
                    windowService.ShowArtifactPreview(previewViewModel);
                    await Task.CompletedTask;
                }
            });
        }
        // Refresh file command
        commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
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

    public override bool CanDelete(JsonDatasourceArtifact artifact)
    {
        return artifact.Parent != null;
    }

    public override void Delete(JsonDatasourceArtifact artifact)
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

    protected override Task OnSelectedInternalAsync(JsonDatasourceArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private void EnsureEditViewModel(JsonDatasourceArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new JsonDatasourceEditViewModel();
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

    private Task ShowPropertiesAsync(JsonDatasourceArtifact datasource)
    {
        EnsureEditViewModel(datasource);
        TreeViewController.ShowArtifactDetailsView(_editViewModel!);
        return Task.CompletedTask;
    }
}
