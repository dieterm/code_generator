namespace CodeGenerator.Domain.Databases.RelationalDatabases
{
    /// <summary>
    /// Registry for relational database definitions
    /// Provides easy access to database implementations
    /// </summary>
    public static class RelationalDatabases
    {
        /// <summary>
        /// Get all available relational databases
        /// </summary>
        public static IEnumerable<RelationalDatabase> All => new RelationalDatabase[]
        {
            MysqlDatabase.Instance,
            SqlServerDatabase.Instance,
            PostgreSqlDatabase.Instance
        };

        /// <summary>
        /// Find a database by ID
        /// </summary>
        public static RelationalDatabase? FindById(string id)
        {
            return All.FirstOrDefault(db => db.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Find a database by name
        /// </summary>
        public static RelationalDatabase? FindByName(string name)
        {
            return All.FirstOrDefault(db => db.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
