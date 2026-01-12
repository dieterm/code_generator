using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.Decorators
{
    /// <summary>
    /// Decorator that marks a column as imported from an existing MySQL database.
    /// </summary>
    public class ExistingMysqlColumnDecorator : ArtifactDecorator
    {
        public const string DECORATOR_KEY = "ExistingMysqlColumn";

        public ExistingMysqlColumnDecorator() 
            : base(DECORATOR_KEY)
        {
        }

        public ExistingMysqlColumnDecorator(ArtifactDecoratorState state)
            : base(state)
        {
        }

        /// <summary>
        /// Original column name when imported from database
        /// </summary>
        public string OriginalColumnName
        {
            get => GetValue<string>(nameof(OriginalColumnName));
            set => SetValue(nameof(OriginalColumnName), value);
        }

        /// <summary>
        /// Original data type when imported from database
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
    }
}
