namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.Services
{
    /// <summary>
    /// Information about a database object (table or view)
    /// </summary>
    public class DatabaseObjectInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Schema { get; set; } = string.Empty;
        public DatabaseObjectType ObjectType { get; set; }

        public string DisplayName => $"{Schema}.{Name}";
    }
}
