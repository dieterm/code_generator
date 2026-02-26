using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using PropertyValueChangedEventArgs = CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.ViewModels.PropertyValueChangedEventArgs;

namespace CodeGenerator.Application.Controllers.Workspace.Datasources;

/// <summary>
/// Controller for .NET Assembly datasource artifacts
/// </summary>
public class DotNetAssemblyDatasourceController : WorkspaceArtifactControllerBase<DotNetAssemblyDatasourceArtifact>
{
    private DotNetAssemblyDatasourceEditViewModel? _editViewModel;

    public DotNetAssemblyDatasourceController(
            OperationExecutor operationExecutor,
        WorkspaceTreeViewController workspaceController,
        ILogger<DotNetAssemblyDatasourceController> logger)
        : base(operationExecutor, workspaceController, logger)
    {
    }

    /// <summary>
    /// Handle Treeview EditLabel complete event
    /// </summary>
    protected override void OnArtifactRenamedInternal(DotNetAssemblyDatasourceArtifact artifact, string oldName, string newName)
    {
        artifact.Name = newName;
    }

    protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DotNetAssemblyDatasourceArtifact artifact)
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

       
        // Refresh assembly command
        commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
        {
            Id = "refresh_assembly",
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

    public override bool CanDelete(DotNetAssemblyDatasourceArtifact artifact)
    {
        return artifact.Parent != null;
    }

    public override void Delete(DotNetAssemblyDatasourceArtifact artifact)
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

    protected override Task OnSelectedInternalAsync(DotNetAssemblyDatasourceArtifact artifact, CancellationToken cancellationToken)
    {
        return ShowPropertiesAsync(artifact);
    }

    private void EnsureEditViewModel(DotNetAssemblyDatasourceArtifact artifact)
    {
        if (_editViewModel == null)
        {
            _editViewModel = new DotNetAssemblyDatasourceEditViewModel();
            _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            _editViewModel.AddTypeRequested += OnAddTypeRequested;
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

    private void OnAddTypeRequested(object? sender, AddTypeEventArgs e)
    {
        if (_editViewModel?.Datasource == null) return;

        var datasource = _editViewModel.Datasource;

        if (e.Table is TableArtifact table)
        {
            datasource.AddChild(table);
            TreeViewController.OnArtifactAdded(datasource, table);
            Logger.LogInformation("Added type {TypeName} to datasource {DatasourceName}",
                table.Name, datasource.Name);
        }
    }

    private Task ShowPropertiesAsync(DotNetAssemblyDatasourceArtifact datasource)
    {
        EnsureEditViewModel(datasource);
        TreeViewController.ShowArtifactDetailsView(_editViewModel!);
        return Task.CompletedTask;
    }
}
