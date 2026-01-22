namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Specifies the action to take when a referenced row is deleted or updated
    /// </summary>
    public enum ForeignKeyAction
    {
        /// <summary>
        /// No action specified (database default behavior)
        /// </summary>
        NoAction = 0,

        /// <summary>
        /// Automatically delete/update rows in the child table
        /// </summary>
        Cascade = 1,

        /// <summary>
        /// Set the foreign key column to NULL
        /// </summary>
        SetNull = 2,

        /// <summary>
        /// Prevent the delete/update if there are referencing rows
        /// </summary>
        Restrict = 3
    }
}
