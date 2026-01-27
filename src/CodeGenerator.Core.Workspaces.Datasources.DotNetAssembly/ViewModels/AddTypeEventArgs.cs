using CodeGenerator.Core.Workspaces.Artifacts.Relational;

namespace CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.ViewModels;

/// <summary>
/// Event args for adding a table from a .NET type
/// </summary>
public class AddTypeEventArgs : EventArgs
{
    public TableArtifact Table { get; }

    public AddTypeEventArgs(TableArtifact table)
    {
        Table = table;
    }
}
