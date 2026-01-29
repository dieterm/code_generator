using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    /// <summary>
    /// Available data field control types for edit views
    /// </summary>
    public enum DataFieldControlType
    {
        /// <summary>Single line text input</summary>
        SingleLineTextField,
        /// <summary>Multi-line text input</summary>
        MultiLineTextField,
        /// <summary>Integer number input</summary>
        IntegerField,
        /// <summary>Decimal number input</summary>
        DecimalField,
        /// <summary>Boolean checkbox</summary>
        BooleanField,
        /// <summary>Date picker</summary>
        DateField,
        /// <summary>Date and time picker</summary>
        DateTimeField,
        /// <summary>Time picker</summary>
        TimeField,
        /// <summary>Combobox/dropdown selection</summary>
        ComboboxField,
        /// <summary>Radio button group</summary>
        RadioButtonField,
        /// <summary>File upload/picker</summary>
        FileField,
        /// <summary>Image upload/display</summary>
        ImageField,
        /// <summary>Rich text editor</summary>
        RichTextField,
        /// <summary>Color picker</summary>
        ColorField,
        /// <summary>Password input</summary>
        PasswordField,
        /// <summary>Email input with validation</summary>
        EmailField,
        /// <summary>Phone number input</summary>
        PhoneField,
        /// <summary>URL input with validation</summary>
        UrlField
    }

    /// <summary>
    /// Represents a field definition within an entity edit view.
    /// Maps a property to a specific UI control type.
    /// </summary>
    public class EntityEditViewFieldArtifact : Artifact, IEditableTreeNode
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
