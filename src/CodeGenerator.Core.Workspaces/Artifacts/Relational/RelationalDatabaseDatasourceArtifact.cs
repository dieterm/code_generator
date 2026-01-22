using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Base class for relational database datasources
    /// </summary>
    public abstract class RelationalDatabaseDatasourceArtifact : DatasourceArtifact, IEditableTreeNode
    {
        //private string _connectionString;

        protected RelationalDatabaseDatasourceArtifact(string name, string? description = null) 
            : base(name, description)
        {
            ConnectionString = string.Empty;
        }

        protected RelationalDatabaseDatasourceArtifact(ArtifactState state)
            : base(state)
        {
        }

        public abstract TemplateDatasourceProviderDecorator CreateTableArtifactTemplateDatasourceProviderDecorator();
        public abstract TemplateDatasourceProviderDecorator CreateViewArtifactTemplateDatasourceProviderDecorator();

        public abstract Domain.Databases.RelationalDatabases.RelationalDatabase GetDomainRelationalDatabase();

        public override string DatasourceCategory => "Relational Database";

        protected override string IconKey => "database";

        /// <summary>
        /// Connection string for the database
        /// </summary>
        public string ConnectionString
        {
            get => GetValue<string>(nameof(ConnectionString));
            set => SetValue(nameof(ConnectionString), value);
        }

        /// <summary>
        /// Get all tables in this database
        /// </summary>
        public IEnumerable<TableArtifact> GetTables() => 
            Children.OfType<TableArtifact>();

        /// <summary>
        /// Get all views in this database
        /// </summary>
        public IEnumerable<ViewArtifact> GetViews() => 
            Children.OfType<ViewArtifact>();

        /// <summary>
        /// Add a table to the database
        /// </summary>
        public TableArtifact AddTable(string tableName)
        {
            var table = new TableArtifact(tableName);
            AddChild(table);
            return table;
        }

        /// <summary>
        /// Add a view to the database
        /// </summary>
        public ViewArtifact AddView(string viewName)
        {
            var view = new ViewArtifact(viewName);
            AddChild(view);
            return view;
        }

        /// <summary>
        /// Remove a table from the database
        /// </summary>
        public void RemoveTable(TableArtifact table)
        {
            RemoveChild(table);
        }

        /// <summary>
        /// Remove a view from the database
        /// </summary>
        public void RemoveView(ViewArtifact view)
        {
            RemoveChild(view);
        }
        public TableArtifact? FindTableByExistingTableName(string originalReferencedTableSchema, string originalReferencedTableName)
        {
            return FindDescendantDecorators<ExistingTableDecorator>()
                .FirstOrDefault(decorator =>
                    string.Equals(decorator.OriginalSchema, originalReferencedTableSchema, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(decorator.OriginalTableName, originalReferencedTableName, StringComparison.OrdinalIgnoreCase))
                ?.Artifact as TableArtifact;
        }

        public void TryCompleteForeignKeys(TableArtifact table, ILogger? logger = null)
        {
            table.GetForeignKeys().ToList().ForEach(fk =>
            {
                if (fk.ReferencedTableId == null)
                {
                    var existingForeignKeyDecorator = fk.GetDecoratorOfType<ExistingForeignKeyDecorator>();
                    if (existingForeignKeyDecorator == null) return;

                    var referencedTable = this.FindTableByExistingTableName(existingForeignKeyDecorator.OriginalReferencedTableSchema, existingForeignKeyDecorator.OriginalReferencedTableName);
                    if (referencedTable != null)
                    {
                        fk.ReferencedTableId = referencedTable.Id;
                        logger?.LogInformation("Completed foreign key {ForeignKeyName} reference to table {ReferencedTableName} in datasource {DatasourceName}", fk.Name, referencedTable.Name, this.Name);

                        foreach (var columnPair in existingForeignKeyDecorator.OriginalColumnMappings)
                        {
                            var fkColumn = table.FindColumnByExistingColumnName(columnPair.SourceColumnName);
                            var pkColumn = referencedTable.FindColumnByExistingColumnName(columnPair.ReferencedColumnName);
                            if (fkColumn != null && pkColumn != null)
                            {
                                var existingMapping = fk.ColumnMappings.FirstOrDefault(cm => cm.SourceColumnId == fkColumn.Id);
                                if (existingMapping != null)
                                {
                                    if (string.IsNullOrWhiteSpace(existingMapping.ReferencedColumnId))
                                    {
                                        existingMapping.ReferencedColumnId = pkColumn.Id;
                                    }

                                }
                                else
                                {
                                    fk.AddColumnMapping(fkColumn.Id, pkColumn.Id);
                                    logger?.LogInformation("Mapped foreign key column {FkColumnName} to primary key column {PkColumnName} for foreign key {ForeignKeyName} in datasource {DatasourceName}", fkColumn.Name, pkColumn.Name, fk.Name, this.Name);
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
