namespace CodeGenerator.Core.Workspaces.Datasources.Json.ViewModels;

/// <summary>
/// Event args for adding a table from JSON
/// </summary>
public class AddTableEventArgs : EventArgs
{
    public object Table { get; }

    public AddTableEventArgs(object table)
    {
        Table = table;
    }
}
