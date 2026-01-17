using CodeGenerator.Core.Workspaces.Datasources.Csv.Services;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Core.Workspaces.Datasources.Csv.ViewModels;

/// <summary>
/// ViewModel for CSV file info display
/// </summary>
public class CsvFileInfoViewModel : ViewModelBase
{
    public string TableName { get; set; } = string.Empty;
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
    public List<string> ColumnNames { get; set; } = new();
    public string DisplayName { get; set; } = string.Empty;

    public string TypeIcon => "table";

    public static CsvFileInfoViewModel FromCsvFileInfo(CsvFileInfo info)
    {
        return new CsvFileInfoViewModel
        {
            TableName = info.TableName,
            RowCount = info.RowCount,
            ColumnCount = info.ColumnCount,
            ColumnNames = info.ColumnNames,
            DisplayName = info.DisplayName
        };
    }
}
