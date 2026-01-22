using CodeGenerator.Core.Artifacts;
using CodeGenerator.Shared.Memento;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Decorator that marks a table as imported from an existing MySQL database.
    /// Stores the original table name and schema for tracking purposes.
    /// </summary>
    public class ExistingTableDecorator : ArtifactDecorator
    {
        public const string DECORATOR_KEY = "ExistingTable";

        public ExistingTableDecorator() 
            : base(DECORATOR_KEY)
        {
        }

        public ExistingTableDecorator(ArtifactDecoratorState state)
            : base(state)
        {
        }

        /// <summary>
        /// Original table name when imported from database
        /// </summary>
        public string OriginalTableName
        {
            get => GetValue<string>(nameof(OriginalTableName));
            set => SetValue(nameof(OriginalTableName), value);
        }

        /// <summary>
        /// Original schema name when imported from database
        /// </summary>
        public string OriginalSchema
        {
            get => GetValue<string>(nameof(OriginalSchema));
            set => SetValue(nameof(OriginalSchema), value);
        }

        /// <summary>
        /// Timestamp when the table was imported
        /// </summary>
        public DateTime ImportedAt
        {
            get => GetValue<DateTime>(nameof(ImportedAt));
            set => SetValue(nameof(ImportedAt), value);
        }

        /// <summary>
        /// Name of the datasource this table was imported from
        /// </summary>
        public string SourceDatasourceName
        {
            get => GetValue<string>(nameof(SourceDatasourceName));
            set => SetValue(nameof(SourceDatasourceName), value);
        }
    }
}
