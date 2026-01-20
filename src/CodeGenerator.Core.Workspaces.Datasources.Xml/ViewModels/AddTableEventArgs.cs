using CodeGenerator.Core.Workspaces.Artifacts.Relational;

namespace CodeGenerator.Core.Workspaces.Datasources.Xml.ViewModels;

/// <summary>
/// Event args for add table requests
/// </summary>
public class AddTableEventArgs : EventArgs
{
    public TableArtifact Table { get; }

    public AddTableEventArgs(TableArtifact table)
    {
        Table = table;
    }
}
