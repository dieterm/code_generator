using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Application.ViewModels.Workspace;

/// <summary>
/// Field model for selecting a TableArtifact from the workspace
/// </summary>
public class TableArtifactFieldModel : FieldViewModelBase
{
    private List<TemplateDatasourceArtifactItem> _availableTables = new();
    private TemplateDatasourceArtifactItem? _selectedTable;

    /// <summary>
    /// List of available tables from the workspace
    /// </summary>
    public List<TemplateDatasourceArtifactItem> AvailableTables
    {
        get => _availableTables;
        set => SetProperty(ref _availableTables, value);
    }

    /// <summary>
    /// Currently selected table
    /// </summary>
    public TemplateDatasourceArtifactItem? SelectedTable
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
            if (value is TemplateDatasourceArtifactItem item)
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
