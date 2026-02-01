using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.OpenApi.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.OpenApi.ViewModels;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Datasources;

/// <summary>
/// Controller for OpenAPI datasource artifacts
/// </summary>
public class OpenApiDatasourceController : ArtifactControllerBase<WorkspaceTreeViewController, OpenApiDatasourceArtifact>
{
    private OpenApiDatasourceEditViewModel? _editViewModel;

    public OpenApiDatasourceController(
        WorkspaceTreeViewController workspaceController,
        ILogger<OpenApiDatasourceController> logger)
        : base(workspaceController, logger)
    {
    }

    /// <summary>
    /// Handle Treeview EditLabel complete event
    /// </summary>
    protected override void OnArtifactRenamedInternal(OpenApiDatasourceArtifact artifact, string oldName, string newName)
    {
        artifact.Name = newName;
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(OpenApiDatasourceArtifact artifact)
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
                    var languageSchema = artifact.FilePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                        ? ViewModels.ArtifactPreviewViewModel.KnownLanguages.JScript
                        : ViewModels.ArtifactPreviewViewModel.KnownLanguages.Text;

                    var previewViewModel = new ViewModels.ArtifactPreviewViewModel
                    {
                        TabLabel = Path.GetFileName(artifact.FilePath),
                        FilePath = artifact.FilePath,
                        TextLanguageSchema = languageSchema
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

    public override bool CanDelete(OpenApiDatasourceArtifact artifact)
    {
        return artifact.Parent != null;
    }

    public override void Delete(OpenApiDatasourceArtifact artifact)
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

    protected override Task OnSelectedInternalAsync(OpenApiDatasourceArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private void EnsureEditViewModel(OpenApiDatasourceArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new OpenApiDatasourceEditViewModel();
            _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            _editViewModel.AddSchemaRequested += OnAddSchemaRequested;
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

    private void OnAddSchemaRequested(object? sender, AddSchemaEventArgs e)
    {
        if (_editViewModel?.Datasource == null) return;

        var datasource = _editViewModel.Datasource;

        if (e.Schema is TableArtifact table)
        {
            datasource.AddChild(table);
            TreeViewController.OnArtifactAdded(datasource, table);
            Logger.LogInformation("Added schema {SchemaName} to datasource {DatasourceName}",
                table.Name, datasource.Name);
        }
    }

    private Task ShowPropertiesAsync(OpenApiDatasourceArtifact datasource)
    {
        EnsureEditViewModel(datasource);
        TreeViewController.ShowArtifactDetailsView(_editViewModel!);
        return Task.CompletedTask;
    }
}
