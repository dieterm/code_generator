using CodeGenerator.Core.Artifacts;
using CodeGenerator.Shared.Memento;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Decorator that marks a foreign key as imported from an existing database.
    /// Stores all original values to detect changes after import.
    /// </summary>
    public class ExistingForeignKeyDecorator : ArtifactDecorator
    {
        public const string DECORATOR_KEY = "ExistingForeignKey";

        public ExistingForeignKeyDecorator() 
            : base(DECORATOR_KEY)
        {
            OriginalColumnMappings = new List<ExistingForeignKeyColumnMapping>();
        }

        public ExistingForeignKeyDecorator(ArtifactDecoratorState state)
            : base(state)
        {
            if (OriginalColumnMappings == null)
            {
                OriginalColumnMappings = new List<ExistingForeignKeyColumnMapping>();
            }
        }

        /// <summary>
        /// Original foreign key constraint name when imported from database
        /// </summary>
        public string OriginalName
        {
            get => GetValue<string>(nameof(OriginalName));
            set => SetValue(nameof(OriginalName), value);
        }

        /// <summary>
        /// Original source table name (the table that contains the foreign key)
        /// </summary>
        public string OriginalSourceTableName
        {
            get => GetValue<string>(nameof(OriginalSourceTableName));
            set => SetValue(nameof(OriginalSourceTableName), value);
        }

        /// <summary>
        /// Original source table schema
        /// </summary>
        public string OriginalSourceTableSchema
        {
            get => GetValue<string>(nameof(OriginalSourceTableSchema));
            set => SetValue(nameof(OriginalSourceTableSchema), value);
        }

        /// <summary>
        /// Original referenced table name
        /// </summary>
        public string OriginalReferencedTableName
        {
            get => GetValue<string>(nameof(OriginalReferencedTableName));
            set => SetValue(nameof(OriginalReferencedTableName), value);
        }

        /// <summary>
        /// Original referenced table schema
        /// </summary>
        public string OriginalReferencedTableSchema
        {
            get => GetValue<string>(nameof(OriginalReferencedTableSchema));
            set => SetValue(nameof(OriginalReferencedTableSchema), value);
        }

        /// <summary>
        /// Original ON DELETE action
        /// </summary>
        public ForeignKeyAction OriginalOnDeleteAction
        {
            get => GetValue<ForeignKeyAction>(nameof(OriginalOnDeleteAction));
            set => SetValue(nameof(OriginalOnDeleteAction), value);
        }

        /// <summary>
        /// Original ON UPDATE action
        /// </summary>
        public ForeignKeyAction OriginalOnUpdateAction
        {
            get => GetValue<ForeignKeyAction>(nameof(OriginalOnUpdateAction));
            set => SetValue(nameof(OriginalOnUpdateAction), value);
        }

        /// <summary>
        /// Original column mappings (source column name to referenced column name)
        /// </summary>
        public List<ExistingForeignKeyColumnMapping> OriginalColumnMappings
        {
            get => GetValue<List<ExistingForeignKeyColumnMapping>>(nameof(OriginalColumnMappings));
            set => SetValue(nameof(OriginalColumnMappings), value);
        }

        /// <summary>
        /// Timestamp when the foreign key was imported
        /// </summary>
        public DateTime ImportedAt
        {
            get => GetValue<DateTime>(nameof(ImportedAt));
            set => SetValue(nameof(ImportedAt), value);
        }

        /// <summary>
        /// Name of the datasource from which this was imported
        /// </summary>
        public string SourceDatasourceName
        {
            get => GetValue<string>(nameof(SourceDatasourceName));
            set => SetValue(nameof(SourceDatasourceName), value);
        }

        public override void RestoreState(IMementoState state)
        {
            base.RestoreState(state);
            FixListOfObject<ExistingForeignKeyColumnMapping>(nameof(OriginalColumnMappings));
        }
    }
}
