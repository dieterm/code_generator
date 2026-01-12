using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.ViewModels;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for MySQL datasource artifacts
    /// </summary>
    public class MysqlDatasourceController : ArtifactControllerBase<MysqlDatasourceArtifact>
    {
        private MysqlDatasourceEditViewModel? _editViewModel;

        public MysqlDatasourceController(
            WorkspaceController workspaceController,
            ILogger<MysqlDatasourceController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override IEnumerable<WorkspaceCommand> GetCommands(MysqlDatasourceArtifact artifact)
        {
            var commands = new List<WorkspaceCommand>();

            // Rename command
            commands.Add(new WorkspaceCommand
            {
                Id = "rename_datasource",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    WorkspaceController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Add table command
            commands.Add(new WorkspaceCommand
            {
                Id = "add_table",
                Text = "Add Table",
                IconKey = "table",
                Execute = async (a) =>
                {
                    var table = new TableArtifact("NewTable");
                    artifact.AddChild(table);
                    WorkspaceController.OnArtifactAdded(artifact, table);
                    await Task.CompletedTask;
                }
            });

            // Add view command
            commands.Add(new WorkspaceCommand
            {
                Id = "add_view",
                Text = "Add View",
                IconKey = "eye",
                Execute = async (a) =>
                {
                    var view = new ViewArtifact("NewView");
                    artifact.AddChild(view);
                    WorkspaceController.OnArtifactAdded(artifact, view);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Refresh schema command
            commands.Add(new WorkspaceCommand
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

            commands.Add(WorkspaceCommand.Separator);

            // Delete command
            commands.Add(new WorkspaceCommand
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
                        WorkspaceController.OnArtifactRemoved(parent, artifact);
                    }
                    await Task.CompletedTask;
                }
            });

            // Properties command
            commands.Add(new WorkspaceCommand
            {
                Id = "datasource_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(MysqlDatasourceArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(MysqlDatasourceArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new MysqlDatasourceEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
                _editViewModel.AddObjectRequested += OnAddObjectRequested;
            }

            _editViewModel.Datasource = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, PropertyValueChangedEventArgs e)
        {
            if (_editViewModel?.Datasource is IArtifact artifact)
            {
                WorkspaceController.OnArtifactPropertyChanged(artifact, e.PropertyName, e.Value);
            }
        }

        private void OnAddObjectRequested(object? sender, AddDatabaseObjectEventArgs e)
        {
            if (_editViewModel?.Datasource == null) return;

            var datasource = _editViewModel.Datasource;

            if (e.DatabaseObject is TableArtifact table)
            {
                datasource.AddChild(table);
                WorkspaceController.OnArtifactAdded(datasource, table);
                Logger.LogInformation("Added table {TableName} to datasource {DatasourceName}", 
                    table.Name, datasource.Name);
            }
            else if (e.DatabaseObject is ViewArtifact view)
            {
                datasource.AddChild(view);
                WorkspaceController.OnArtifactAdded(datasource, view);
                Logger.LogInformation("Added view {ViewName} to datasource {DatasourceName}", 
                    view.Name, datasource.Name);
            }
        }

        private Task ShowPropertiesAsync(MysqlDatasourceArtifact datasource)
        {
            EnsureEditViewModel(datasource);
            WorkspaceController.ShowWorkspaceDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
