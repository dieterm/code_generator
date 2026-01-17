namespace CodeGenerator.Core.Workspaces.Datasources.Csv.Services;

/// <summary>
/// Information about a CSV file structure
/// </summary>
public class CsvFileInfo
{
    public string FileName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
    public List<string> ColumnNames { get; set; } = new();

    public string DisplayName => $"{TableName} ({ColumnCount} columns, {RowCount} rows)";
}
