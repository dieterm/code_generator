using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.Decorators
{
    /// <summary>
    /// Decorator that marks an index as imported from an existing MySQL database.
    /// </summary>
    public class ExistingMysqlIndexDecorator : ArtifactDecorator
    {
        public const string DECORATOR_KEY = "ExistingMysqlIndex";

        public ExistingMysqlIndexDecorator() 
            : base(DECORATOR_KEY)
        {
        }

        public ExistingMysqlIndexDecorator(ArtifactDecoratorState state)
            : base(state)
        {
        }

        /// <summary>
        /// Original index name when imported from database
        /// </summary>
        public string OriginalIndexName
        {
            get => GetValue<string>(nameof(OriginalIndexName));
            set => SetValue(nameof(OriginalIndexName), value);
        }

        /// <summary>
        /// Original index type (e.g., BTREE, HASH)
        /// </summary>
        public string OriginalIndexType
        {
            get => GetValue<string>(nameof(OriginalIndexType));
            set => SetValue(nameof(OriginalIndexType), value);
        }
    }
}
