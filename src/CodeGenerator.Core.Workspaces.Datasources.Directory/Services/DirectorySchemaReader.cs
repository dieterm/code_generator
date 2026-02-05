using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Directory.Models;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.Directory.Services;

/// <summary>
/// Service for reading schema from directory structures
/// </summary>
public class DirectorySchemaReader
{
    /// <summary>
    /// Get information about a directory
    /// </summary>
    public Task<DirectoryInfo?> GetDirectoryInfoAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(directoryPath) || !System.IO.Directory.Exists(directoryPath))
        {
            return Task.FromResult<DirectoryInfo?>(null);
        }

        return Task.FromResult<DirectoryInfo?>(new DirectoryInfo(directoryPath));
    }

    /// <summary>
    /// Create a DirectoryModel from a directory path
    /// </summary>
    public Task<DirectoryModel?> LoadDirectoryModelAsync(
        string directoryPath, 
        string? searchPattern = null,
        bool includeSubdirectories = true,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(directoryPath) || !System.IO.Directory.Exists(directoryPath))
        {
            return Task.FromResult<DirectoryModel?>(null);
        }

        var directoryInfo = new DirectoryInfo(directoryPath);
        var model = DirectoryModel.FromDirectoryInfo(directoryInfo, directoryPath, searchPattern, includeSubdirectories);

        return Task.FromResult<DirectoryModel?>(model);
    }

    /// <summary>
    /// Import a directory structure as tables with columns
    /// </summary>
    public Task<TableArtifact> ImportDirectoryAsync(
        string directoryPath,
        string datasourceName,
        string? searchPattern = null,
        CancellationToken cancellationToken = default)
    {
        var directoryName = Path.GetFileName(directoryPath) ?? "Directory";
        var table = new TableArtifact(directoryName, string.Empty);

        // Add decorator to mark as existing
        table.AddDecorator(new ExistingTableDecorator
        {
            OriginalTableName = directoryName,
            OriginalSchema = string.Empty,
            ImportedAt = DateTime.UtcNow,
            SourceDatasourceName = datasourceName
        });

        // Add directory columns
        AddDirectoryColumns(table);

        // Add Files nested table
        var filesColumn = new ColumnArtifact("Files", GenericDataTypes.Json.Id, true)
        {
            OrdinalPosition = table.Children.Count() + 1
        };
        filesColumn.AddDecorator(new ExistingColumnDecorator
        {
            OriginalName = "Files",
            OriginalDataType = GenericDataTypes.Json.Id,
            OriginalOrdinalPosition = filesColumn.OrdinalPosition,
            OriginalIsNullable = true
        });
        table.AddChild(filesColumn);

        // Add Files sub-table
        var filesTable = new TableArtifact("File", string.Empty);
        AddFileColumns(filesTable);
        filesColumn.AddChild(filesTable);

        // Add Directories nested table (recursive structure)
        var directoriesColumn = new ColumnArtifact("Directories", GenericDataTypes.Json.Id, true)
        {
            OrdinalPosition = table.Children.Count() + 1
        };
        directoriesColumn.AddDecorator(new ExistingColumnDecorator
        {
            OriginalName = "Directories",
            OriginalDataType = GenericDataTypes.Json.Id,
            OriginalOrdinalPosition = directoriesColumn.OrdinalPosition,
            OriginalIsNullable = true
        });
        table.AddChild(directoriesColumn);

        // Add Directories sub-table (same structure as parent - recursive)
        var directoriesTable = new TableArtifact("Directory", string.Empty);
        AddDirectoryColumns(directoriesTable);
        directoriesColumn.AddChild(directoriesTable);

        return Task.FromResult(table);
    }

    private void AddDirectoryColumns(TableArtifact table)
    {
        int ordinal = 1;

        AddColumn(table, "Name", GenericDataTypes.VarChar.Id, false, ordinal++);
        AddColumn(table, "FullPath", GenericDataTypes.VarChar.Id, false, ordinal++);
        AddColumn(table, "RelativePath", GenericDataTypes.VarChar.Id, false, ordinal++);
        AddColumn(table, "ParentDirectory", GenericDataTypes.VarChar.Id, true, ordinal++);
        AddColumn(table, "CreationDate", GenericDataTypes.DateTime.Id, false, ordinal++);
        AddColumn(table, "ModifiedDate", GenericDataTypes.DateTime.Id, false, ordinal++);
        AddColumn(table, "IsHidden", GenericDataTypes.Boolean.Id, false, ordinal++);
        AddColumn(table, "IsSystem", GenericDataTypes.Boolean.Id, false, ordinal++);
        AddColumn(table, "Attributes", GenericDataTypes.VarChar.Id, true, ordinal++);
        AddColumn(table, "Size", GenericDataTypes.BigInt.Id, false, ordinal++);
        AddColumn(table, "FileCount", GenericDataTypes.Int.Id, false, ordinal++);
        AddColumn(table, "DirectoryCount", GenericDataTypes.Int.Id, false, ordinal++);
    }

    private void AddFileColumns(TableArtifact table)
    {
        int ordinal = 1;

        AddColumn(table, "Title", GenericDataTypes.VarChar.Id, false, ordinal++);
        AddColumn(table, "Extension", GenericDataTypes.VarChar.Id, false, ordinal++);
        AddColumn(table, "Name", GenericDataTypes.VarChar.Id, false, ordinal++);
        AddColumn(table, "FullPath", GenericDataTypes.VarChar.Id, false, ordinal++);
        AddColumn(table, "Directory", GenericDataTypes.VarChar.Id, false, ordinal++);
        AddColumn(table, "RelativePath", GenericDataTypes.VarChar.Id, false, ordinal++);
        AddColumn(table, "Size", GenericDataTypes.BigInt.Id, false, ordinal++);
        AddColumn(table, "CreationDate", GenericDataTypes.DateTime.Id, false, ordinal++);
        AddColumn(table, "ModifiedDate", GenericDataTypes.DateTime.Id, false, ordinal++);
        AddColumn(table, "LastAccessDate", GenericDataTypes.DateTime.Id, false, ordinal++);
        AddColumn(table, "IsReadOnly", GenericDataTypes.Boolean.Id, false, ordinal++);
        AddColumn(table, "IsHidden", GenericDataTypes.Boolean.Id, false, ordinal++);
        AddColumn(table, "IsSystem", GenericDataTypes.Boolean.Id, false, ordinal++);
        AddColumn(table, "Attributes", GenericDataTypes.VarChar.Id, true, ordinal++);
    }

    private void AddColumn(TableArtifact table, string name, string dataType, bool isNullable, int ordinal)
    {
        var column = new ColumnArtifact(name, dataType, isNullable)
        {
            OrdinalPosition = ordinal
        };
        column.AddDecorator(new ExistingColumnDecorator
        {
            OriginalName = name,
            OriginalDataType = dataType,
            OriginalOrdinalPosition = ordinal,
            OriginalIsNullable = isNullable
        });
        table.AddChild(column);
    }

    /// <summary>
    /// Get summary information about a directory
    /// </summary>
    public DirectorySummary GetDirectorySummary(string directoryPath, string? searchPattern = null)
    {
        var summary = new DirectorySummary
        {
            DirectoryPath = directoryPath,
            Exists = System.IO.Directory.Exists(directoryPath)
        };

        if (!summary.Exists)
            return summary;

        try
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            summary.DirectoryName = directoryInfo.Name;
            summary.CreationDate = directoryInfo.CreationTime;
            summary.ModifiedDate = directoryInfo.LastWriteTime;

            var pattern = string.IsNullOrEmpty(searchPattern) ? "*.*" : searchPattern;

            // Count files matching the pattern
            summary.FileCount = directoryInfo.GetFiles(pattern, SearchOption.TopDirectoryOnly).Length;
            summary.TotalFileCount = directoryInfo.GetFiles(pattern, SearchOption.AllDirectories).Length;

            // Count directories
            summary.DirectoryCount = directoryInfo.GetDirectories().Length;
            summary.TotalDirectoryCount = directoryInfo.GetDirectories("*", SearchOption.AllDirectories).Length;

            // Calculate total size
            summary.TotalSize = directoryInfo.GetFiles(pattern, SearchOption.AllDirectories)
                .Sum(f => f.Length);
        }
        catch (UnauthorizedAccessException)
        {
            summary.AccessError = "Access denied to some directories";
        }
        catch (Exception ex)
        {
            summary.AccessError = ex.Message;
        }

        return summary;
    }
}

/// <summary>
/// Summary information about a directory
/// </summary>
public class DirectorySummary
{
    public string DirectoryPath { get; set; } = string.Empty;
    public string DirectoryName { get; set; } = string.Empty;
    public bool Exists { get; set; }
    public int FileCount { get; set; }
    public int TotalFileCount { get; set; }
    public int DirectoryCount { get; set; }
    public int TotalDirectoryCount { get; set; }
    public long TotalSize { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string? AccessError { get; set; }

    public string TotalSizeFormatted => FormatFileSize(TotalSize);

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}
