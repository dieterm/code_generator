using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.ViewModels;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Datasources
{
    /// <summary>
    /// Controller for MySQL datasource artifacts
    /// </summary>
    public class MysqlDatasourceController : ArtifactControllerBase<WorkspaceTreeViewController, MysqlDatasourceArtifact>
    {
        private MysqlDatasourceEditViewModel? _editViewModel;

        public MysqlDatasourceController(
            WorkspaceTreeViewController workspaceController,
            ILogger<MysqlDatasourceController> logger)
            : base(workspaceController, logger)
        {
            
        }
        /// <summary>
        /// Handle Treeview EditLabel complete event
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        protected override void OnArtifactRenamedInternal(MysqlDatasourceArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(MysqlDatasourceArtifact artifact)
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

            // Add table command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "add_table",
                Text = "Add Table",
                IconKey = "table",
                Execute = async (a) =>
                {
                    var table = new TableArtifact("NewTable");
                    artifact.AddChild(table);
                    TreeViewController.OnArtifactAdded(artifact, table);
                    await Task.CompletedTask;
                }
            });

            // Add view command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "add_view",
                Text = "Add View",
                IconKey = "eye",
                Execute = async (a) =>
                {
                    var view = new ViewArtifact("NewView");
                    artifact.AddChild(view);
                    TreeViewController.OnArtifactAdded(artifact, view);
                    await Task.CompletedTask;
                }
            });

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Refresh schema command
            commands.Add(new ArtifactTreeNodeCommand
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
                datasource.TryCompleteForeignKeys(table, Logger);
                TreeViewController.OnArtifactAdded(datasource, table);
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

        //private void TryCompleteForeignKeys(TableArtifact table, MysqlDatasourceArtifact datasource)
        //{
        //    table.GetForeignKeys().ToList().ForEach(fk =>
        //    {
        //        if (fk.ReferencedTableId == null)
        //        {
        //            var existingForeignKeyDecorator = fk.GetDecoratorOfType<ExistingForeignKeyDecorator>();
        //            if (existingForeignKeyDecorator == null) return;

        //            var referencedTable = datasource.FindTableByExistingTableName(existingForeignKeyDecorator.OriginalReferencedTableSchema, existingForeignKeyDecorator.OriginalReferencedTableName);
        //            if (referencedTable != null)
        //            {
        //                fk.ReferencedTableId = referencedTable.Id;
        //                Logger.LogInformation("Completed foreign key {ForeignKeyName} reference to table {ReferencedTableName} in datasource {DatasourceName}", fk.Name, referencedTable.Name, datasource.Name);

        //                foreach (var columnPair in existingForeignKeyDecorator.OriginalColumnMappings)
        //                {
        //                    var fkColumn = table.FindColumnByExistingColumnName(columnPair.SourceColumnName);
        //                    var pkColumn = referencedTable.FindColumnByExistingColumnName(columnPair.ReferencedColumnName);
        //                    if (fkColumn != null && pkColumn != null)
        //                    {
        //                        var existingMapping = fk.ColumnMappings.FirstOrDefault(cm => cm.SourceColumnId == fkColumn.Id);
        //                        if (existingMapping != null)
        //                        {
        //                            if(string.IsNullOrWhiteSpace(existingMapping.ReferencedColumnId))
        //                            {
        //                                existingMapping.ReferencedColumnId = pkColumn.Id;
        //                            }
                                    
        //                        } else { 
        //                            fk.AddColumnMapping(fkColumn.Id, pkColumn.Id);
        //                            Logger.LogInformation("Mapped foreign key column {FkColumnName} to primary key column {PkColumnName} for foreign key {ForeignKeyName} in datasource {DatasourceName}",fkColumn.Name, pkColumn.Name, fk.Name, datasource.Name);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    });
        //}

        private Task ShowPropertiesAsync(MysqlDatasourceArtifact datasource)
        {
            EnsureEditViewModel(datasource);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
