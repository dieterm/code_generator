using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.Decorators
{
    /// <summary>
    /// Decorator that marks a view as imported from an existing MySQL database.
    /// </summary>
    public class ExistingMysqlViewDecorator : ArtifactDecorator
    {
        public const string DECORATOR_KEY = "ExistingMysqlView";

        public ExistingMysqlViewDecorator() 
            : base(DECORATOR_KEY)
        {
        }

        public ExistingMysqlViewDecorator(ArtifactDecoratorState state)
            : base(state)
        {
        }

        /// <summary>
        /// Original view name when imported from database
        /// </summary>
        public string OriginalViewName
        {
            get => GetValue<string>(nameof(OriginalViewName));
            set => SetValue(nameof(OriginalViewName), value);
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
        /// Timestamp when the view was imported
        /// </summary>
        public DateTime ImportedAt
        {
            get => GetValue<DateTime>(nameof(ImportedAt));
            set => SetValue(nameof(ImportedAt), value);
        }

        /// <summary>
        /// Name of the datasource this view was imported from
        /// </summary>
        public string SourceDatasourceName
        {
            get => GetValue<string>(nameof(SourceDatasourceName));
            set => SetValue(nameof(SourceDatasourceName), value);
        }
    }
}
