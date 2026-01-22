using ClosedXML.Excel;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.Excel.Services;

/// <summary>
/// Service for reading schema and data from Excel files
/// </summary>
public class ExcelSchemaReader
{
    /// <summary>
    /// Get all sheets from an Excel file
    /// </summary>
    public Task<List<SheetInfo>> GetSheetsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var result = new List<SheetInfo>();

        if (!File.Exists(filePath))
            return Task.FromResult(result);

        using var workbook = new XLWorkbook(filePath);
        
        foreach (var worksheet in workbook.Worksheets)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var usedRange = worksheet.RangeUsed();
            result.Add(new SheetInfo
            {
                Name = worksheet.Name,
                RowCount = usedRange?.RowCount() ?? 0,
                ColumnCount = usedRange?.ColumnCount() ?? 0
            });
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Import a sheet as a table with its columns
    /// </summary>
    public Task<TableArtifact> ImportSheetAsync(
        string filePath,
        string sheetName,
        string datasourceName,
        bool firstRowIsHeader = true,
        CancellationToken cancellationToken = default)
    {
        var table = new TableArtifact(sheetName, string.Empty);

        // Add decorator to mark as existing
        table.AddDecorator(new ExistingTableDecorator
        {
            OriginalTableName = sheetName,
            OriginalSchema = string.Empty,
            ImportedAt = DateTime.UtcNow,
            SourceDatasourceName = datasourceName
        });

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(sheetName);
        
        if (worksheet == null)
            throw new ArgumentException($"Sheet '{sheetName}' not found in workbook");

        var usedRange = worksheet.RangeUsed();
        if (usedRange == null)
            return Task.FromResult(table);

        var firstRow = usedRange.FirstRow();
        var columnCount = usedRange.ColumnCount();

        // Import columns
        for (int col = 1; col <= columnCount; col++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string columnName;
            string inferredType;

            if (firstRowIsHeader)
            {
                var headerCell = firstRow.Cell(col);
                columnName = headerCell.GetString();
                if (string.IsNullOrWhiteSpace(columnName))
                    columnName = $"Column{col}";

                // Infer type from data in the column (skip header row)
                inferredType = InferColumnType(worksheet, col, 2, usedRange.RowCount());
            }
            else
            {
                columnName = $"Column{col}";
                inferredType = InferColumnType(worksheet, col, 1, usedRange.RowCount());
            }

            var column = new ColumnArtifact(columnName, inferredType, true)
            {
                OrdinalPosition = col
            };

            // Add decorator to mark as existing
            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = columnName,
                OriginalDataType = inferredType,
                OriginalOrdinalPosition = col,
                OriginalIsNullable = true
            });

            table.AddChild(column);
        }

        return Task.FromResult(table);
    }

    /// <summary>
    /// Infer the data type of a column by examining its values
    /// </summary>
    private string InferColumnType(IXLWorksheet worksheet, int columnIndex, int startRow, int totalRows)
    {
        bool hasNumbers = false;
        bool hasDecimals = false;
        bool hasDates = false;
        bool hasBooleans = false;
        bool hasStrings = false;
        int sampleSize = Math.Min(100, totalRows - startRow + 1);

        for (int row = startRow; row < startRow + sampleSize && row <= totalRows; row++)
        {
            var cell = worksheet.Cell(row, columnIndex);
            
            if (cell.IsEmpty())
                continue;

            var cellType = cell.DataType;

            switch (cellType)
            {
                case XLDataType.Number:
                    hasNumbers = true;
                    if (cell.GetDouble() != Math.Floor(cell.GetDouble()))
                        hasDecimals = true;
                    break;
                case XLDataType.DateTime:
                    hasDates = true;
                    break;
                case XLDataType.Boolean:
                    hasBooleans = true;
                    break;
                case XLDataType.Text:
                default:
                    hasStrings = true;
                    break;
            }
        }

        // Determine type based on what was found
        if (hasStrings)
            return GenericDataTypes.Text.Id;
        if (hasDates)
            return GenericDataTypes.DateTime.Id;
        if (hasBooleans)
            return GenericDataTypes.Boolean.Id;
        if (hasDecimals)
            return GenericDataTypes.Decimal.Id;
        if (hasNumbers)
            return GenericDataTypes.Int.Id;
        return GenericDataTypes.Text.Id; // Default
    }

    /// <summary>
    /// Read data from a sheet
    /// </summary>
    public Task<List<Dictionary<string, object?>>> ReadSheetDataAsync(
        string filePath,
        string sheetName,
        bool firstRowIsHeader = true,
        int? maxRows = null,
        CancellationToken cancellationToken = default)
    {
        var results = new List<Dictionary<string, object?>>();

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(sheetName);
        
        if (worksheet == null)
            return Task.FromResult(results);

        var usedRange = worksheet.RangeUsed();
        if (usedRange == null)
            return Task.FromResult(results);

        var columnCount = usedRange.ColumnCount();
        var rowCount = usedRange.RowCount();

        // Get column names
        var columnNames = new List<string>();
        var dataStartRow = 1;

        if (firstRowIsHeader)
        {
            var headerRow = usedRange.FirstRow();
            for (int col = 1; col <= columnCount; col++)
            {
                var name = headerRow.Cell(col).GetString();
                columnNames.Add(string.IsNullOrWhiteSpace(name) ? $"Column{col}" : name);
            }
            dataStartRow = 2;
        }
        else
        {
            for (int col = 1; col <= columnCount; col++)
            {
                columnNames.Add($"Column{col}");
            }
        }

        // Read data rows
        var rowsToRead = maxRows.HasValue 
            ? Math.Min(maxRows.Value, rowCount - dataStartRow + 1) 
            : rowCount - dataStartRow + 1;

        for (int rowIdx = 0; rowIdx < rowsToRead; rowIdx++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var row = new Dictionary<string, object?>();
            var worksheetRow = worksheet.Row(dataStartRow + rowIdx);

            for (int col = 0; col < columnCount; col++)
            {
                var cell = worksheetRow.Cell(col + 1);
                row[columnNames[col]] = cell.IsEmpty() ? null : cell.Value.ToString();
            }

            results.Add(row);
        }

        return Task.FromResult(results);
    }
}
