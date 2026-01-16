using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

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
    }
}
