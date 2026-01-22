using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    /// <summary>
    /// Represents a property of an entity state
    /// </summary>
    public class PropertyArtifact : Artifact, IEditableTreeNode
    {
        public PropertyArtifact(string name, string dataType, bool isNullable = true)
            : base()
        {
            Name = name;
            DataType = dataType;
            IsNullable = isNullable;
        }

        public PropertyArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon => GetTreeNodeIcon();

        private ResourceManagerTreeNodeIcon GetTreeNodeIcon()
        {
            if (DataType == GenericDataTypes.Guid.Id)
                return new ResourceManagerTreeNodeIcon("square-asterisk");
            else if (DataType == GenericDataTypes.Xml.Id)
                return new ResourceManagerTreeNodeIcon("code-xml");
            else if (DataType == GenericDataTypes.Json.Id)
                return new ResourceManagerTreeNodeIcon("braces");
            else if (DataType == GenericDataTypes.Money.Id)
                return new ResourceManagerTreeNodeIcon("dollar-sign");
            else if (GenericDataTypes.IsTextBasedType(DataType))
                return new ResourceManagerTreeNodeIcon("case-sensitive");
            else if (GenericDataTypes.IsNumericType(DataType))
                return new ResourceManagerTreeNodeIcon("hash");
            else if (GenericDataTypes.IsDateType(DataType))
                return new ResourceManagerTreeNodeIcon("calendar");
            else if (GenericDataTypes.IsBinaryType(DataType))
                return new ResourceManagerTreeNodeIcon("binary");
            else if (GenericDataTypes.IsBooleanType(DataType))
                return new ResourceManagerTreeNodeIcon("toggle-left");
            else
                return new ResourceManagerTreeNodeIcon("circle-question-mark");
        }

        /// <summary>
        /// Property name
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
        /// Data type (generic data type id)
        /// </summary>
        public string DataType
        {
            get => GetValue<string>(nameof(DataType));
            set
            {
                if (SetValue(nameof(DataType), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeIcon));
            }
        }

        /// <summary>
        /// Is the property nullable
        /// </summary>
        public bool IsNullable
        {
            get => GetValue<bool>(nameof(IsNullable));
            set => SetValue(nameof(IsNullable), value);
        }

        /// <summary>
        /// Maximum length for string types
        /// </summary>
        public int? MaxLength
        {
            get => GetValue<int?>(nameof(MaxLength));
            set => SetValue(nameof(MaxLength), value);
        }

        /// <summary>
        /// Precision for decimal types
        /// </summary>
        public int? Precision
        {
            get => GetValue<int?>(nameof(Precision));
            set => SetValue(nameof(Precision), value);
        }

        /// <summary>
        /// Scale for decimal types
        /// </summary>
        public int? Scale
        {
            get => GetValue<int?>(nameof(Scale));
            set => SetValue(nameof(Scale), value);
        }

        /// <summary>
        /// Property description for documentation purposes
        /// </summary>
        public string? Description
        {
            get => GetValue<string?>(nameof(Description));
            set => SetValue(nameof(Description), value);
        }

        /// <summary>
        /// Example value for documentation purposes
        /// </summary>
        public string? ExampleValue
        {
            get => GetValue<string?>(nameof(ExampleValue));
            set => SetValue(nameof(ExampleValue), value);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
