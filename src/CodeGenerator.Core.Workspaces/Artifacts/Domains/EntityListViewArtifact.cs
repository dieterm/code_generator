using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    /// <summary>
    /// Represents a list view definition for an entity.
    /// Defines which columns to show in a data grid (e.g., Syncfusion SfDataGrid).
    /// Uses properties from the entity's DefaultState and relations.
    /// </summary>
    public class EntityListViewArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public EntityListViewArtifact(string name)
            : base()
        {
            Name = name;
        }

        public EntityListViewArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => $"List: {Name}";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("table");

        /// <summary>
        /// View name
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
        /// Get all column definitions for this list view
        /// </summary>
        public IEnumerable<EntityListViewColumnArtifact> GetColumns() =>
            Children.OfType<EntityListViewColumnArtifact>();

        /// <summary>
        /// Add a column definition to this list view
        /// </summary>
        public void AddColumn(EntityListViewColumnArtifact column)
        {
            AddChild(column);
        }

        /// <summary>
        /// Remove a column definition from this list view
        /// </summary>
        public void RemoveColumn(EntityListViewColumnArtifact column)
        {
            RemoveChild(column);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
