using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Xml.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Xml.ViewModels;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Datasources;

/// <summary>
/// Controller for XML datasource artifacts
/// </summary>
public class XmlDatasourceController : ArtifactControllerBase<WorkspaceTreeViewController, XmlDatasourceArtifact>
{
    private XmlDatasourceEditViewModel? _editViewModel;

    public XmlDatasourceController(
        WorkspaceTreeViewController workspaceController,
        ILogger<XmlDatasourceController> logger)
        : base(workspaceController, logger)
    {
    }

    /// <summary>
    /// Handle Treeview EditLabel complete event
    /// </summary>
    protected override void OnArtifactRenamedInternal(XmlDatasourceArtifact artifact, string oldName, string newName)
    {
        artifact.Name = newName;
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(XmlDatasourceArtifact artifact)
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
        // View/Edit file command
        if (System.IO.File.Exists(artifact.FilePath))
        {
            commands.Add(new ArtifactTreeNodeCommand
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
                        TextLanguageSchema = ViewModels.ArtifactPreviewViewModel.KnownLanguages.XML
                    };
                    windowService.ShowArtifactPreview(previewViewModel);
                    await Task.CompletedTask;
                }
            });
        }
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

    protected override Task OnSelectedInternalAsync(XmlDatasourceArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private void EnsureEditViewModel(XmlDatasourceArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new XmlDatasourceEditViewModel();
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

    private Task ShowPropertiesAsync(XmlDatasourceArtifact datasource)
    {
        EnsureEditViewModel(datasource);
        TreeViewController.ShowArtifactDetailsView(_editViewModel!);
        return Task.CompletedTask;
    }
}
