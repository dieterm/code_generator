using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Memento;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Represents a database index
    /// </summary>
    public class IndexArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public IndexArtifact(string name, bool isUnique = false)
        {
            Name = name;
            IsUnique = isUnique;
            IsClustered = false;
            ColumnNames = new List<string>();
        }

        public IndexArtifact(ArtifactState state)
            : base(state)
        {
            if (ColumnNames == null)
            {
                ColumnNames = new List<string>();
            }
        }

        //public override string Id => $"index_{Name}".ToLowerInvariant();

        public override string TreeNodeText { get { return IsUnique ? $"{Name} (Unique)" : Name; } }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("list");

        /// <summary>
        /// Index name
        /// </summary>
        public string Name
        {
            get => GetValue<string>(nameof(Name));
            set => SetValue(nameof(Name), value);
        }

        /// <summary>
        /// Is this a unique index
        /// </summary>
        public bool IsUnique
        {
            get => GetValue<bool>(nameof(IsUnique));
            set => SetValue(nameof(IsUnique), value);
        }

        /// <summary>
        /// Is this a clustered index
        /// </summary>
        public bool IsClustered
        {
            get => GetValue<bool>(nameof(IsClustered));
            set => SetValue(nameof(IsClustered), value);
        }

        /// <summary>
        /// Column names included in the index
        /// </summary>
        public List<string> ColumnNames
        {
            get => GetValue<List<string>>(nameof(ColumnNames));
            set => SetValue(nameof(ColumnNames), value);
        }

        /// <summary>
        /// Add a column to the index
        /// </summary>
        public void AddColumn(string columnName)
        {
            if (!ColumnNames.Contains(columnName))
            {
                ColumnNames.Add(columnName);
            }
        }

        /// <summary>
        /// Remove a column from the index
        /// </summary>
        public void RemoveColumn(string columnName)
        {
            ColumnNames.Remove(columnName);
        }

        public override void RestoreState(IMementoState state)
        {
            base.RestoreState(state);

            // JSON-fix: convert ColumnNames from List<object> to List<string>
            FixListOfObject<string>(nameof(ColumnNames));
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
}
