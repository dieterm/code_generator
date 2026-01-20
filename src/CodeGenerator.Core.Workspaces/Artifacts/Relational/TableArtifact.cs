using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Views.TreeNode;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    
    /// <summary>
    /// Represents a database table
    /// </summary>
    public class TableArtifact : Artifact, IEditableTreeNode
    {
        public const string DEFAULT_SCHEMA = "";

        public TableArtifact(string name, string schema = DEFAULT_SCHEMA)
        {
            Name = name;
            Schema = schema;
            RemovedExistingColumns = new List<string>();
            RemovedExistingIndexes = new List<string>();
        }
        public TableArtifact(ArtifactState state)
            : base(state)
        {
            FixListOfObject<string>(nameof(RemovedExistingColumns));
            FixListOfObject<string>(nameof(RemovedExistingIndexes));
            if (RemovedExistingColumns == null)
                RemovedExistingColumns = new List<string>();
            if(RemovedExistingIndexes == null)
                RemovedExistingIndexes = new List<string>();
        }

        public override string TreeNodeText => Name;

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
                SetValue(nameof(Schema), value);
            }
        }

        public List<string> RemovedExistingColumns
        {
            get => GetValue<List<string>>(nameof(RemovedExistingColumns));
            set => SetValue(nameof(RemovedExistingColumns), value);
        }

        public List<string> RemovedExistingIndexes
        {
            get => GetValue<List<string>>(nameof(RemovedExistingIndexes));
            set => SetValue(nameof(RemovedExistingIndexes), value);
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
        /// Get all foreign keys
        /// </summary>
        public IEnumerable<ForeignKeyArtifact> GetForeignKeys() =>
            Children.OfType<ForeignKeyArtifact>();

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
        /// Add a foreign key to the table
        /// </summary>
        public ForeignKeyArtifact AddForeignKey(string foreignKeyName)
        {
            var foreignKey = new ForeignKeyArtifact(foreignKeyName);
            AddChild(foreignKey);
            return foreignKey;
        }

        public override void RemoveChild(IArtifact child)
        {
            base.RemoveChild(child);
            if(child is ColumnArtifact column)
            {
                if(column.HasDecorator<ExistingColumnDecorator>())
                    RemovedExistingColumns.Add(column.Name);
            }
            else if(child is IndexArtifact index)
            {
                if(index.HasDecorator<ExistingIndexDecorator>())
                    RemovedExistingIndexes.Add(index.Name);
            }
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

        /// <summary>
        /// Remove a foreign key from the table
        /// </summary>
        public void RemoveForeignKey(ForeignKeyArtifact foreignKey)
        {
            RemoveChild(foreignKey);
        }

        public IEnumerable<ColumnArtifact> GetForeignKeyColumns()
        {
            return GetColumns().Where(c => c.IsForeignKey);
        }

        /// <summary>
        /// Return the columns that don't have an ExistingColumnDecorator
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ColumnArtifact> GetNewColumns()
        {
            return GetColumns().Where(c => !c.HasDecorator<ExistingColumnDecorator>());
        }

        public IEnumerable<ColumnArtifact> GetExistingColumns()
        {
            return GetColumns().Where(c => c.HasDecorator<ExistingColumnDecorator>());
        }

        public IEnumerable<ColumnArtifact> GetModifiedExistingColumns()
        {
            return GetExistingColumns().Where(c => c.HasExistingChanges());
        }

        public bool CanAlterTable()
        {
            return HasDecorator<ExistingTableDecorator>() &&(RemovedExistingColumns.Count>0 || RemovedExistingIndexes.Count>0 ||  HasExistingNameChanged() || (GetNewColumns().Any() || GetModifiedExistingColumns().Any()));
        }

        public bool HasExistingNameChanged()
        {
            var existingTableDecorator = GetDecoratorOfType<ExistingTableDecorator>();
            if (existingTableDecorator == null)
                return false;
            return !string.Equals(existingTableDecorator.OriginalTableName, Name, StringComparison.OrdinalIgnoreCase);
        }
        public bool CanBeginEdit()
        {
            return true;
        }

        public bool Validating(string newName)
        {
            return !string.IsNullOrWhiteSpace(newName);
        }

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }

        public virtual TemplateDatasourceProviderDecorator? GetTemplateDatasourceProviderDecorator()
        {
            return GetDecoratorOfType<TemplateDatasourceProviderDecorator>();
        }
    }
}
