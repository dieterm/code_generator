using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Represents a database column
    /// </summary>
    public class ColumnArtifact : Artifact, IEditableTreeNode
    {
        public ColumnArtifact(string name, string dataType, bool isNullable = true)
        {
            Name = name;
            DataType = dataType;
            IsNullable = isNullable;
        }

        public ColumnArtifact(ArtifactState state)
            : base(state)
        {
            
        }

        public override string TreeNodeText { get { return Name;} }

        public override ITreeNodeIcon TreeNodeIcon
        { 
            get { 
                return GetTreeNodeIcon();
            }
        }

        private ResourceManagerTreeNodeIcon GetTreeNodeIcon()
        {
            if (IsPrimaryKey)
                return new ResourceManagerTreeNodeIcon("key-round");
            else if (IsForeignKey)
                return new ResourceManagerTreeNodeIcon("link");
            else if (DataType == GenericDataTypes.Guid.Id)
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
        /// Column name
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(nameof(Name)); }
            set { 
                SetValue<string>(nameof(Name), value);
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Data type (e.g., "int", "varchar", "datetime")
        /// </summary>
        public string DataType
        {
            get { return GetValue<string>(nameof(DataType)); }
            set { 
                SetValue<string>(nameof(DataType), value);
                RaisePropertyChangedEvent(nameof(TreeNodeIcon));
            }
        }

        /// <summary>
        /// Ordinal position of the column within the table
        public int OrdinalPosition
        {
            get { return GetValue<int>(nameof(OrdinalPosition)); }
            set { 
                SetValue<int>(nameof(OrdinalPosition), value);
            }
        }

        /// <summary>
        /// Is the column nullable
        /// </summary>
        public bool IsNullable
        {
            get { return GetValue<bool>(nameof(IsNullable)); }
            set { 
                SetValue<bool>(nameof(IsNullable), value);
                //RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Is this column part of the primary key
        /// </summary>
        public bool IsPrimaryKey
        {
            get { return GetValue<bool>(nameof(IsPrimaryKey)); }
            set { 
                SetValue<bool>(nameof(IsPrimaryKey), value);
                //RaisePropertyChangedEvent(nameof(TreeNodeText));
                RaisePropertyChangedEvent(nameof(TreeNodeIcon));
            }
        }

        /// <summary>
        /// Is this an auto-increment/identity column
        /// </summary>
        public bool IsAutoIncrement
        {
            get { return GetValue<bool>(nameof(IsAutoIncrement)); }
            set { 
                SetValue<bool>(nameof(IsAutoIncrement), value);
                //RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Maximum length for string types
        /// </summary>
        public int? MaxLength
        {
            get { return GetValue<int?>(nameof(MaxLength)); }
            set { 
                SetValue<int?>(nameof(MaxLength), value);
                //RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Precision for decimal types
        /// </summary>
        public int? Precision
        {
            get { return GetValue<int?>(nameof(Precision)); }
            set { 
                SetValue<int?>(nameof(Precision), value);
                //RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Scale for decimal types
        /// </summary>
        public int? Scale
        {
            get { return GetValue<int?>(nameof(Scale)); }
            set { 
                SetValue<int?>(nameof(Scale), value);
                //RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Default value expression
        /// </summary>
        public string? DefaultValue
        {
            get { return GetValue<string?>(nameof(DefaultValue)); }
            set { 
                SetValue<string?>(nameof(DefaultValue), value);
                //RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Foreign key reference table name
        /// </summary>
        public string? ForeignKeyTable
        {
            get { return GetValue<string?>(nameof(ForeignKeyTable)); }
            set { 
                SetValue<string?>(nameof(ForeignKeyTable), value);
                //RaisePropertyChangedEvent(nameof(TreeNodeText));
                RaisePropertyChangedEvent(nameof(IsForeignKey));
                RaisePropertyChangedEvent(nameof(TreeNodeIcon));
            }
        }

        /// <summary>
        /// Foreign key reference column name
        /// </summary>
        public string? ForeignKeyColumn
        {
            get { return GetValue<string?>(nameof(ForeignKeyColumn)); }
            set { 
                SetValue<string?>(nameof(ForeignKeyColumn), value);
                //RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Is this a foreign key column
        /// </summary>
        public bool IsForeignKey => !string.IsNullOrEmpty(ForeignKeyTable);

        public bool HasExistingChanges()
        {
            var decorator = GetDecorator<ExistingColumnDecorator>();
            if(decorator == null)
                return false;

            return 
                IsPropertyDirty(decorator,nameof(Name)) ||
                IsPropertyDirty(decorator,nameof(DataType)) ||
                IsPropertyDirty(decorator,nameof(OrdinalPosition)) ||
                IsPropertyDirty(decorator,nameof(IsNullable)) ||
                IsPropertyDirty(decorator,nameof(IsPrimaryKey)) ||
                IsPropertyDirty(decorator,nameof(IsAutoIncrement)) ||
                IsPropertyDirty(decorator,nameof(MaxLength)) ||
                IsPropertyDirty(decorator,nameof(Precision)) ||
                IsPropertyDirty(decorator,nameof(Scale)) ||
                IsPropertyDirty(decorator,nameof(DefaultValue)) ||
                IsPropertyDirty(decorator,nameof(ForeignKeyTable)) ||
                IsPropertyDirty(decorator,nameof(ForeignKeyColumn));
        }

        private bool IsPropertyDirty(ExistingColumnDecorator decorator, string propertyName)
        {
            var orignalValue = decorator.GetType().GetProperty("Original" + propertyName)?.GetValue(decorator);
            var currentValue = this.GetType().GetProperty(propertyName)?.GetValue(this);
            return !Equals(orignalValue, currentValue);
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
