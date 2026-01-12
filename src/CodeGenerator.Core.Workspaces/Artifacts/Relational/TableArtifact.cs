using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    
    /// <summary>
    /// Represents a database table
    /// </summary>
    public class TableArtifact : Artifact, IEditableTreeNode
    {
        public const string DEFAULT_SCHEMA = "dbo";
        //private string _name;
        //private string _schema;

        public TableArtifact(string name, string schema = DEFAULT_SCHEMA)
        {
            Name = name;
            Schema = schema;
        }
        public TableArtifact(ArtifactState state)
            : base(state)
        {
        }
        //public override string Id => $"table_{Schema}_{Name}".ToLowerInvariant();

        public override string TreeNodeText => string.IsNullOrEmpty(Schema) || Schema == DEFAULT_SCHEMA
            ? Name 
            : $"{Schema}.{Name}";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("table");

        /// <summary>
        /// Table name
        /// </summary>
        public string Name
        {
            get => GetValue<string>(nameof(Name));
            set { 
                if(SetValue(nameof(Name), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Schema name (e.g., "dbo")
        /// </summary>
        public string Schema
        {
            get => GetValue<string>(nameof(Schema));
            set { 
                if(SetValue(nameof(Schema), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Get all columns
        /// </summary>
        public IEnumerable<ColumnArtifact> GetColumns() => 
            Children.OfType<ColumnArtifact>();

        /// <summary>
        /// Get all indexes
        /// </summary>
        public IEnumerable<IndexArtifact> GetIndexes() => 
            Children.OfType<IndexArtifact>();

        /// <summary>
        /// Get the primary key columns
        /// </summary>
        public IEnumerable<ColumnArtifact> GetPrimaryKeyColumns() => 
            GetColumns().Where(c => c.IsPrimaryKey);

        /// <summary>
        /// Add a column to the table
        /// </summary>
        public ColumnArtifact AddColumn(string columnName, string dataType, bool isNullable = true)
        {
            var column = new ColumnArtifact(columnName, dataType, isNullable);
            AddChild(column);
            return column;
        }

        /// <summary>
        /// Add an index to the table
        /// </summary>
        public IndexArtifact AddIndex(string indexName, bool isUnique = false)
        {
            var index = new IndexArtifact(indexName, isUnique);
            AddChild(index);
            return index;
        }

        /// <summary>
        /// Remove a column from the table
        /// </summary>
        public void RemoveColumn(ColumnArtifact column)
        {
            RemoveChild(column);
        }

        /// <summary>
        /// Remove an index from the table
        /// </summary>
        public void RemoveIndex(IndexArtifact index)
        {
            RemoveChild(index);
        }
    }
}
