using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Application.ViewModels.Workspace;

/// <summary>
/// Field model for selecting a TableArtifact from the workspace
/// </summary>
public class TableArtifactFieldModel : FieldViewModelBase
{
    private List<TableArtifactItem> _availableTables = new();
    private TableArtifactItem? _selectedTable;

    /// <summary>
    /// List of available tables from the workspace
    /// </summary>
    public List<TableArtifactItem> AvailableTables
    {
        get => _availableTables;
        set => SetProperty(ref _availableTables, value);
    }

    /// <summary>
    /// Currently selected table
    /// </summary>
    public TableArtifactItem? SelectedTable
    {
        get => _selectedTable;
        set
        {
            if (SetProperty(ref _selectedTable, value))
            {
                // Update the base Value property
                SetValue(value);
            }
        }
    }

    public override object Value
    {
        get => _selectedTable;
        set
        {
            if (value is TableArtifactItem item)
            {
                SelectedTable = item;
            }
            else if (value == null)
            {
                SelectedTable = null;
            }
        }
    }
}

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
