using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{

    /// <summary>
    /// Represents a field definition within an entity edit view.
    /// Maps a property to a specific UI control type.
    /// </summary>
    public class EntityEditViewFieldArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public EntityEditViewFieldArtifact(string propertyName)
            : base()
        {
            PropertyName = propertyName;
        }

        public EntityEditViewFieldArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => $"{PropertyName} ({ControlType})";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("text-cursor-input");

        /// <summary>
        /// The property name this field is bound to
        /// </summary>
        public string PropertyName
        {
            get => GetValue<string>(nameof(PropertyName));
            set
            {
                if (SetValue(nameof(PropertyName), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// The UI control type to use for this field
        /// </summary>
        public DataFieldControlType ControlType
        {
            get => GetValue<DataFieldControlType>(nameof(ControlType));
            set
            {
                if (SetValue(nameof(ControlType), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Custom label to display (if different from property name)
        /// </summary>
        public string? Label
        {
            get => GetValue<string?>(nameof(Label));
            set => SetValue(nameof(Label), value);
        }

        /// <summary>
        /// Tooltip/help text for this field
        /// </summary>
        public string? Tooltip
        {
            get => GetValue<string?>(nameof(Tooltip));
            set => SetValue(nameof(Tooltip), value);
        }

        /// <summary>
        /// Whether this field is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get => GetValue<bool>(nameof(IsReadOnly));
            set => SetValue(nameof(IsReadOnly), value);
        }

        /// <summary>
        /// Whether this field is required
        /// </summary>
        public bool IsRequired
        {
            get => GetValue<bool>(nameof(IsRequired));
            set => SetValue(nameof(IsRequired), value);
        }

        /// <summary>
        /// Display order (lower numbers appear first)
        /// </summary>
        public int DisplayOrder
        {
            get => GetValue<int>(nameof(DisplayOrder));
            set => SetValue(nameof(DisplayOrder), value);
        }

        /// <summary>
        /// Placeholder text for text-based controls
        /// </summary>
        public string? Placeholder
        {
            get => GetValue<string?>(nameof(Placeholder));
            set => SetValue(nameof(Placeholder), value);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            PropertyName = newName;
        }
    }
}
