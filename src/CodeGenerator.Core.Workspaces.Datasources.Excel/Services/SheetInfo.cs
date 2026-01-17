namespace CodeGenerator.Core.Workspaces.Datasources.Excel.Services;

/// <summary>
/// Information about an Excel sheet (treated as a table)
/// </summary>
public class SheetInfo
{
    public string Name { get; set; } = string.Empty;
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }

    public string DisplayName => $"{Name} ({ColumnCount} columns, {RowCount} rows)";
}
