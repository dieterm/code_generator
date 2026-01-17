namespace CodeGenerator.Core.Workspaces.Datasources.Csv.ViewModels;

/// <summary>
/// Event args for adding a table from CSV
/// </summary>
public class AddTableEventArgs : EventArgs
{
    public object Table { get; }

    public AddTableEventArgs(object table)
    {
        Table = table;
    }
}
