using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    /// <summary>
    /// Represents a select view definition for an entity.
    /// Defines which properties to use for DisplayMember and ValueMember in a combobox.
    /// </summary>
    public class EntitySelectViewArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public EntitySelectViewArtifact(string name)
            : base()
        {
            Name = name;
        }

        public EntitySelectViewArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => $"Select: {Name}";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("chevrons-up-down");

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
        /// Property path to use as DisplayMember (what the user sees)
        /// Can be a simple property or a formatted expression.
        /// </summary>
        public string? DisplayMemberPath
        {
            get => GetValue<string?>(nameof(DisplayMemberPath));
            set => SetValue(nameof(DisplayMemberPath), value);
        }

        /// <summary>
        /// Property path to use as ValueMember (the actual value stored)
        /// Typically the entity's ID property.
        /// </summary>
        public string? ValueMemberPath
        {
            get => GetValue<string?>(nameof(ValueMemberPath));
            set => SetValue(nameof(ValueMemberPath), value);
        }

        /// <summary>
        /// Optional display format template.
        /// E.g., "{FirstName} {LastName}" for combining multiple properties.
        /// </summary>
        public string? DisplayFormat
        {
            get => GetValue<string?>(nameof(DisplayFormat));
            set => SetValue(nameof(DisplayFormat), value);
        }

        /// <summary>
        /// Optional filter/search property path for autocomplete functionality
        /// </summary>
        public string? SearchPropertyPath
        {
            get => GetValue<string?>(nameof(SearchPropertyPath));
            set => SetValue(nameof(SearchPropertyPath), value);
        }

        /// <summary>
        /// Optional sort property path for ordering items in the dropdown
        /// </summary>
        public string? SortPropertyPath
        {
            get => GetValue<string?>(nameof(SortPropertyPath));
            set => SetValue(nameof(SortPropertyPath), value);
        }

        /// <summary>
        /// Whether to sort ascending (true) or descending (false)
        /// </summary>
        public bool SortAscending
        {
            get => GetValue<bool>(nameof(SortAscending));
            set => SetValue(nameof(SortAscending), value);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
