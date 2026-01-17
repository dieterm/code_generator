using CodeGenerator.Core.Workspaces.Datasources.Excel.Services;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Core.Workspaces.Datasources.Excel.ViewModels;

/// <summary>
/// ViewModel for a sheet in the list
/// </summary>
public class SheetViewModel : ViewModelBase
{
    public string Name { get; set; } = string.Empty;
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
    public string DisplayName { get; set; } = string.Empty;

    public string TypeIcon => "table";

    public static SheetViewModel FromSheetInfo(SheetInfo info)
    {
        return new SheetViewModel
        {
            Name = info.Name,
            RowCount = info.RowCount,
            ColumnCount = info.ColumnCount,
            DisplayName = info.DisplayName
        };
    }
}
