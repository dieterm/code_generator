namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Represents an original column mapping in a foreign key (by column names, not IDs)
    /// </summary>
    public class ExistingForeignKeyColumnMapping
    {
        public ExistingForeignKeyColumnMapping()
        {
            SourceColumnName = string.Empty;
            ReferencedColumnName = string.Empty;
        }

        public ExistingForeignKeyColumnMapping(string sourceColumnName, string referencedColumnName)
        {
            SourceColumnName = sourceColumnName;
            ReferencedColumnName = referencedColumnName;
        }

        /// <summary>
        /// The name of the source column (in the table that owns the foreign key)
        /// </summary>
        public string SourceColumnName { get; set; }

        /// <summary>
        /// The name of the referenced column (in the referenced table)
        /// </summary>
        public string ReferencedColumnName { get; set; }
    }
}
