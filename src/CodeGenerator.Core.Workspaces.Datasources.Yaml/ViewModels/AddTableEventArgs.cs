namespace CodeGenerator.Core.Workspaces.Datasources.Yaml.ViewModels;

/// <summary>
/// Event args for adding a table from YAML
/// </summary>
public class AddTableEventArgs : EventArgs
{
    public object Table { get; }

    public AddTableEventArgs(object table)
    {
        Table = table;
    }
}
