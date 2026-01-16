using CodeGenerator.Core.Workspaces.Artifacts.Relational;

namespace CodeGenerator.Application.ViewModels.Workspace;

/// <summary>
/// Item representing a TableArtifact for selection in a combobox
/// </summary>
public class TableArtifactItem
{
    /// <summary>
    /// The TableArtifact
    /// </summary>
    public TableArtifact TableArtifact { get; set; } = null!;

    /// <summary>
    /// Display name including datasource name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Full path: Datasource > Schema > Table
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// The datasource artifact this table belongs to
    /// </summary>
    public Core.Workspaces.Artifacts.DatasourceArtifact? DatasourceArtifact { get; set; }

    public override string ToString() => DisplayName;
}
