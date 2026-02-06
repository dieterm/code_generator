using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities
{

    /// <summary>
    /// Represents a column definition within an entity list view.
    /// </summary>
    public class EntityListViewColumnArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public EntityListViewColumnArtifact(string propertyPath)
            : base()
        {
            PropertyPath = propertyPath;
        }

        public EntityListViewColumnArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => HeaderText ?? PropertyPath;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("columns-3");

        /// <summary>
        /// The property path this column is bound to.
        /// Can be a simple property name or a path like "Relation.Property" for related entities.
        /// </summary>
        public string PropertyPath
        {
            get => GetValue<string>(nameof(PropertyPath));
            set
            {
                if (SetValue(nameof(PropertyPath), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Column header text
        /// </summary>
        public string? HeaderText
        {
            get => GetValue<string?>(nameof(HeaderText));
            set
            {
                if (SetValue(nameof(HeaderText), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Column type for rendering
        /// </summary>
        public ListViewColumnType ColumnType
        {
            get => GetValue<ListViewColumnType>(nameof(ColumnType));
            set => SetValue(nameof(ColumnType), value);
        }

        /// <summary>
        /// Column width (null for auto)
        /// </summary>
        public int? Width
        {
            get => GetValue<int?>(nameof(Width));
            set => SetValue(nameof(Width), value);
        }

        /// <summary>
        /// Display order (lower numbers appear first/left)
        /// </summary>
        public int DisplayOrder
        {
            get => GetValue<int>(nameof(DisplayOrder));
            set => SetValue(nameof(DisplayOrder), value);
        }

        /// <summary>
        /// Whether this column is sortable
        /// </summary>
        public bool IsSortable
        {
            get => GetValue<bool>(nameof(IsSortable));
            set => SetValue(nameof(IsSortable), value);
        }

        /// <summary>
        /// Whether this column is filterable
        /// </summary>
        public bool IsFilterable
        {
            get => GetValue<bool>(nameof(IsFilterable));
            set => SetValue(nameof(IsFilterable), value);
        }

        /// <summary>
        /// Whether this column is visible
        /// </summary>
        public bool IsVisible
        {
            get => GetValue<bool>(nameof(IsVisible));
            set => SetValue(nameof(IsVisible), value);
        }

        /// <summary>
        /// Format string for numeric/date columns
        /// </summary>
        public string? FormatString
        {
            get => GetValue<string?>(nameof(FormatString));
            set => SetValue(nameof(FormatString), value);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            PropertyPath = newName;
        }
    }
}
