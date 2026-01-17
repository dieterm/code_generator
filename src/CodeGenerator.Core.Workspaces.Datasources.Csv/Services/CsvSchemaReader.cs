using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.DataTypes;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace CodeGenerator.Core.Workspaces.Datasources.Csv.Services;

/// <summary>
/// Service for reading schema and data from CSV files
/// </summary>
public class CsvSchemaReader
{
    /// <summary>
    /// Get information about a CSV file
    /// </summary>
    public Task<CsvFileInfo> GetFileInfoAsync(
        string filePath, 
        string fieldDelimiter = ",",
        string rowTerminator = "\n",
        bool firstRowIsHeader = true,
        CancellationToken cancellationToken = default)
    {
        var result = new CsvFileInfo
        {
            FileName = Path.GetFileName(filePath),
            TableName = Path.GetFileNameWithoutExtension(filePath)
        };

        if (!File.Exists(filePath))
            return Task.FromResult(result);

        var config = CreateCsvConfiguration(fieldDelimiter, rowTerminator, firstRowIsHeader);

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        // Read header or first row
        if (csv.Read())
        {
            if (firstRowIsHeader)
            {
                csv.ReadHeader();
                result.ColumnNames = csv.HeaderRecord?.ToList() ?? new List<string>();
                result.ColumnCount = result.ColumnNames.Count;
            }
            else
            {
                // Count columns from first data row
                result.ColumnCount = csv.Parser.Count;
                for (int i = 0; i < result.ColumnCount; i++)
                {
                    result.ColumnNames.Add($"Column{i + 1}");
                }
            }
        }

        // Count rows
        int rowCount = firstRowIsHeader ? 0 : 1; // If no header, first row is data
        while (csv.Read())
        {
            cancellationToken.ThrowIfCancellationRequested();
            rowCount++;
        }
        result.RowCount = rowCount;

        return Task.FromResult(result);
    }

    /// <summary>
    /// Import a CSV file as a table with its columns
    /// </summary>
    public Task<TableArtifact> ImportCsvAsync(
        string filePath,
        string datasourceName,
        string fieldDelimiter = ",",
        string rowTerminator = "\n",
        bool firstRowIsHeader = true,
        CancellationToken cancellationToken = default)
    {
        var tableName = Path.GetFileNameWithoutExtension(filePath);
        var table = new TableArtifact(tableName, string.Empty);

        // Add decorator to mark as existing
        table.AddDecorator(new ExistingTableDecorator
        {
            OriginalTableName = tableName,
            OriginalSchema = string.Empty,
            ImportedAt = DateTime.UtcNow,
            SourceDatasourceName = datasourceName
        });

        if (!File.Exists(filePath))
            return Task.FromResult(table);

        var config = CreateCsvConfiguration(fieldDelimiter, rowTerminator, firstRowIsHeader);

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        // Get column names
        List<string> columnNames = new();
        
        if (csv.Read())
        {
            if (firstRowIsHeader)
            {
                csv.ReadHeader();
                columnNames = csv.HeaderRecord?.ToList() ?? new List<string>();
            }
            else
            {
                // Generate column names
                for (int i = 0; i < csv.Parser.Count; i++)
                {
                    columnNames.Add($"Column{i + 1}");
                }
            }
        }

        // Collect sample data for type inference
        var sampleData = new List<string[]>();
        int sampleSize = 100;
        int rowsRead = 0;

        // If no header, include first row in sample
        if (!firstRowIsHeader && csv.Parser.Record != null)
        {
            sampleData.Add(csv.Parser.Record);
            rowsRead++;
        }

        while (csv.Read() && rowsRead < sampleSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csv.Parser.Record != null)
            {
                sampleData.Add(csv.Parser.Record);
                rowsRead++;
            }
        }

        // Create columns with inferred types
        for (int col = 0; col < columnNames.Count; col++)
        {
            var columnName = columnNames[col];
            if (string.IsNullOrWhiteSpace(columnName))
                columnName = $"Column{col + 1}";

            var inferredType = InferColumnType(sampleData, col);

            var column = new ColumnArtifact(columnName, inferredType, true)
            {
                OrdinalPosition = col + 1
            };

            // Add decorator to mark as existing
            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = columnName,
                OriginalDataType = inferredType,
                OriginalOrdinalPosition = col + 1,
                OriginalIsNullable = true
            });

            table.AddChild(column);
        }

        return Task.FromResult(table);
    }

    /// <summary>
    /// Read data from a CSV file
    /// </summary>
    public Task<List<Dictionary<string, object?>>> ReadCsvDataAsync(
        string filePath,
        string fieldDelimiter = ",",
        string rowTerminator = "\n",
        bool firstRowIsHeader = true,
        int? maxRows = null,
        CancellationToken cancellationToken = default)
    {
        var results = new List<Dictionary<string, object?>>();

        if (!File.Exists(filePath))
            return Task.FromResult(results);

        var config = CreateCsvConfiguration(fieldDelimiter, rowTerminator, firstRowIsHeader);

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        // Get column names
        List<string> columnNames = new();
        
        if (csv.Read())
        {
            if (firstRowIsHeader)
            {
                csv.ReadHeader();
                columnNames = csv.HeaderRecord?.ToList() ?? new List<string>();
            }
            else
            {
                // Generate column names and add first row as data
                for (int i = 0; i < csv.Parser.Count; i++)
                {
                    columnNames.Add($"Column{i + 1}");
                }
                
                // Add first row as data
                var firstRow = new Dictionary<string, object?>();
                for (int i = 0; i < columnNames.Count && i < csv.Parser.Count; i++)
                {
                    firstRow[columnNames[i]] = csv.Parser.Record?[i];
                }
                results.Add(firstRow);
            }
        }

        // Read data rows
        int rowsRead = results.Count;
        while (csv.Read() && (!maxRows.HasValue || rowsRead < maxRows.Value))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var row = new Dictionary<string, object?>();
            for (int i = 0; i < columnNames.Count; i++)
            {
                row[columnNames[i]] = i < csv.Parser.Count ? csv.Parser.Record?[i] : null;
            }
            results.Add(row);
            rowsRead++;
        }

        return Task.FromResult(results);
    }

    private CsvConfiguration CreateCsvConfiguration(string fieldDelimiter, string rowTerminator, bool hasHeader)
    {
        // Parse escape sequences
        var delimiter = ParseEscapeSequences(fieldDelimiter);
        var newLine = ParseEscapeSequences(rowTerminator);

        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = delimiter,
            NewLine = newLine,
            HasHeaderRecord = hasHeader,
            MissingFieldFound = null,
            BadDataFound = null
        };
    }

    private string ParseEscapeSequences(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input
            .Replace("\\n", "\n")
            .Replace("\\r", "\r")
            .Replace("\\t", "\t")
            .Replace("\\\\", "\\");
    }

    /// <summary>
    /// Infer the data type of a column by examining sample values
    /// </summary>
    private string InferColumnType(List<string[]> sampleData, int columnIndex)
    {
        bool hasNumbers = false;
        bool hasDecimals = false;
        bool hasDates = false;
        bool hasBooleans = false;
        bool hasStrings = false;
        int nonEmptyCount = 0;

        foreach (var row in sampleData)
        {
            if (columnIndex >= row.Length)
                continue;

            var value = row[columnIndex];
            
            if (string.IsNullOrWhiteSpace(value))
                continue;

            nonEmptyCount++;

            // Try boolean
            if (bool.TryParse(value, out _) || 
                value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                value == "0" || value == "1")
            {
                hasBooleans = true;
                continue;
            }

            // Try integer
            if (long.TryParse(value, out _))
            {
                hasNumbers = true;
                continue;
            }

            // Try decimal
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _) ||
                decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out _))
            {
                hasDecimals = true;
                continue;
            }

            // Try date
            if (DateTime.TryParse(value, out _))
            {
                hasDates = true;
                continue;
            }

            // Default to string
            hasStrings = true;
        }

        // If no data, default to string
        if (nonEmptyCount == 0)
            return GenericDataTypes.Text.Id;

        // Determine type based on what was found (strings take precedence)
        if (hasStrings)
            return GenericDataTypes.Text.Id;
        if (hasDates && !hasNumbers && !hasDecimals && !hasBooleans)
            return GenericDataTypes.DateTime.Id;
        if (hasBooleans && !hasNumbers && !hasDecimals && !hasDates)
            return GenericDataTypes.Boolean.Id;
        if (hasDecimals)
            return GenericDataTypes.Decimal.Id;
        if (hasNumbers)
            return GenericDataTypes.Int.Id;

        return GenericDataTypes.Text.Id; // Default
    }
}
