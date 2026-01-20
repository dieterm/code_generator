using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Memento;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Represents a database foreign key constraint
    /// </summary>
    public class ForeignKeyArtifact : Artifact, IEditableTreeNode
    {
        public ForeignKeyArtifact(string name)
        {
            Name = name;
            ColumnMappings = new List<ForeignKeyColumnMapping>();
        }

        public ForeignKeyArtifact(ArtifactState state)
            : base(state)
        {
            if (ColumnMappings == null)
            {
                ColumnMappings = new List<ForeignKeyColumnMapping>();
            }
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("link");

        /// <summary>
        /// Foreign key constraint name
        /// </summary>
        public string Name
        {
            get => GetValue<string>(nameof(Name));
            set
            {
                if (SetValue(nameof(Name), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// The Id of the referenced table
        /// </summary>
        public string? ReferencedTableId
        {
            get => GetValue<string?>(nameof(ReferencedTableId));
            set => SetValue(nameof(ReferencedTableId), value);
        }

        /// <summary>
        /// Column mappings (source column id to referenced column id)
        /// </summary>
        public List<ForeignKeyColumnMapping> ColumnMappings
        {
            get => GetValue<List<ForeignKeyColumnMapping>>(nameof(ColumnMappings));
            set => SetValue(nameof(ColumnMappings), value);
        }

        /// <summary>
        /// Add a column mapping
        /// </summary>
        public void AddColumnMapping(string sourceColumnId, string referencedColumnId)
        {
            var mapping = new ForeignKeyColumnMapping(sourceColumnId, referencedColumnId);
            if (!ColumnMappings.Any(m => m.SourceColumnId == sourceColumnId))
            {
                ColumnMappings.Add(mapping);
            }
        }

        /// <summary>
        /// Remove a column mapping by source column id
        /// </summary>
        public void RemoveColumnMapping(string sourceColumnId)
        {
            var mapping = ColumnMappings.FirstOrDefault(m => m.SourceColumnId == sourceColumnId);
            if (mapping != null)
            {
                ColumnMappings.Remove(mapping);
            }
        }

        /// <summary>
        /// Remove all column mappings that reference the specified table
        /// </summary>
        public void RemoveColumnMappingsForTable(string tableId)
        {
            if (ReferencedTableId == tableId)
            {
                ColumnMappings.Clear();
            }
        }

        /// <summary>
        /// Remove column mappings that reference a specific column
        /// </summary>
        public void RemoveColumnMappingsForColumn(string columnId)
        {
            ColumnMappings.RemoveAll(m => m.SourceColumnId == columnId || m.ReferencedColumnId == columnId);
        }

        /// <summary>
        /// Check if this foreign key references the specified table
        /// </summary>
        public bool ReferencesTable(string tableId)
        {
            return ReferencedTableId == tableId;
        }

        public override void RestoreState(IMementoState state)
        {
            base.RestoreState(state);

            // JSON-fix: convert ColumnMappings from List<object> to List<ForeignKeyColumnMapping>
            FixListOfObject<ForeignKeyColumnMapping>(nameof(ColumnMappings));
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
    }

    /// <summary>
    /// Represents a column mapping in a foreign key
    /// </summary>
    public class ForeignKeyColumnMapping
    {
        public ForeignKeyColumnMapping()
        {
            SourceColumnId = string.Empty;
            ReferencedColumnId = string.Empty;
        }

        public ForeignKeyColumnMapping(string sourceColumnId, string referencedColumnId)
        {
            SourceColumnId = sourceColumnId;
            ReferencedColumnId = referencedColumnId;
        }

        /// <summary>
        /// The Id of the source column (in the table that owns the foreign key)
        /// </summary>
        public string SourceColumnId { get; set; }

        /// <summary>
        /// The Id of the referenced column (in the referenced table)
        /// </summary>
        public string ReferencedColumnId { get; set; }
    }
}
