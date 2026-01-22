using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Decorator that marks a column as imported from an existing MySQL database.
    /// </summary>
    public class ExistingColumnDecorator : ArtifactDecorator
    {
        public const string DECORATOR_KEY = "ExistingColumn";

        public ExistingColumnDecorator() 
            : base(DECORATOR_KEY)
        {
        }

        public ExistingColumnDecorator(ArtifactDecoratorState state)
            : base(state)
        {
        }

        /// <summary>
        /// Original column name when imported from database
        /// </summary>
        public string OriginalName
        {
            get => GetValue<string>(nameof(OriginalName));
            set => SetValue(nameof(OriginalName), value);
        }

        /// <summary>
        /// Original data type when imported from database
        /// Data type (e.g., "int", "varchar", "datetime")
        /// </summary>
        public string OriginalDataType
        {
            get => GetValue<string>(nameof(OriginalDataType));
            set => SetValue(nameof(OriginalDataType), value);
        }

        /// <summary>
        /// Original ordinal position in the table
        /// </summary>
        public int OriginalOrdinalPosition
        {
            get => GetValue<int>(nameof(OriginalOrdinalPosition));
            set => SetValue(nameof(OriginalOrdinalPosition), value);
        }

        /// <summary>
        /// Is the column nullable
        /// </summary>
        public bool OriginalIsNullable
        {
            get { return GetValue<bool>(nameof(OriginalIsNullable)); }
            set
            {
                SetValue<bool>(nameof(OriginalIsNullable), value);
            }
        }

        /// <summary>
        /// Is this column part of the primary key
        /// </summary>
        public bool OriginalIsPrimaryKey
        {
            get { return GetValue<bool>(nameof(OriginalIsPrimaryKey)); }
            set
            {
                SetValue<bool>(nameof(OriginalIsPrimaryKey), value);
            }
        }

        /// <summary>
        /// Is this an auto-increment/identity column
        /// </summary>
        public bool OriginalIsAutoIncrement
        {
            get { return GetValue<bool>(nameof(OriginalIsAutoIncrement)); }
            set
            {
                SetValue<bool>(nameof(OriginalIsAutoIncrement), value);
            }
        }

        /// <summary>
        /// Maximum length for string types
        /// </summary>
        public int? OriginalMaxLength
        {
            get { return GetValue<int?>(nameof(OriginalMaxLength)); }
            set
            {
                SetValue<int?>(nameof(OriginalMaxLength), value);
            }
        }

        /// <summary>
        /// Precision for decimal types
        /// </summary>
        public int? OriginalPrecision
        {
            get { return GetValue<int?>(nameof(OriginalPrecision)); }
            set
            {
                SetValue<int?>(nameof(OriginalPrecision), value);
            }
        }

        /// <summary>
        /// Scale for decimal types
        /// </summary>
        public int? OriginalScale
        {
            get { return GetValue<int?>(nameof(OriginalScale)); }
            set
            {
                SetValue<int?>(nameof(OriginalScale), value);
            }
        }

        /// <summary>
        /// Default value expression
        /// </summary>
        public string? OriginalDefaultValue
        {
            get { return GetValue<string?>(nameof(OriginalDefaultValue)); }
            set
            {
                SetValue<string?>(nameof(OriginalDefaultValue), value);
            }
        }

        /// <summary>
        /// Foreign key reference table name
        /// </summary>
        public string? OriginalForeignKeyTable
        {
            get { return GetValue<string?>(nameof(OriginalForeignKeyTable)); }
            set
            {
                SetValue<string?>(nameof(OriginalForeignKeyTable), value);
            }
        }

        /// <summary>
        /// Foreign key reference column name
        /// </summary>
        public string? OriginalForeignKeyColumn
        {
            get { return GetValue<string?>(nameof(OriginalForeignKeyColumn)); }
            set
            {
                SetValue<string?>(nameof(OriginalForeignKeyColumn), value);
                //RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Is this a foreign key column
        /// </summary>
        public bool OriginalIsForeignKey => !string.IsNullOrEmpty(OriginalForeignKeyTable);
    }
}
