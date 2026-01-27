using CodeGenerator.Core.Workspaces.Artifacts.Relational;

namespace CodeGenerator.Core.Workspaces.Datasources.OpenApi.ViewModels;

/// <summary>
/// Event args for adding a schema from OpenAPI
/// </summary>
public class AddSchemaEventArgs : EventArgs
{
    public TableArtifact Schema { get; }

    public AddSchemaEventArgs(TableArtifact schema)
    {
        Schema = schema;
    }
}
