using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Base class for relational database datasources
    /// </summary>
    public abstract class RelationalDatabaseDatasourceArtifact : DatasourceArtifact, IEditableTreeNode
    {
        private string _connectionString;

        protected RelationalDatabaseDatasourceArtifact(string name) : base(name)
        {
            _connectionString = string.Empty;
        }

        public override string DatasourceCategory => "Relational Database";

        protected override string IconKey => "database";

        /// <summary>
        /// Connection string for the database
        /// </summary>
        public string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
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
