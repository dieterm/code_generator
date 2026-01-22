namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.ViewModels
{
    /// <summary>
    /// Event args for adding a database object
    /// </summary>
    public class AddDatabaseObjectEventArgs : EventArgs
    {
        public object DatabaseObject { get; }

        public AddDatabaseObjectEventArgs(object databaseObject)
        {
            DatabaseObject = databaseObject;
        }
    }
}
