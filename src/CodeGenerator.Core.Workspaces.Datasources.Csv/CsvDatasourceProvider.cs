using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Csv.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.Csv;

/// <summary>
/// Provider for CSV file datasources
/// </summary>
public class CsvDatasourceProvider : IDatasourceProvider
{
    public string TypeId => CsvDatasourceArtifact.TYPE_ID;

    public DatasourceTypeInfo GetTypeInfo()
    {
        return new DatasourceTypeInfo
        {
            TypeId = TypeId,
            DisplayName = "CSV File",
            Category = "File",
            Description = "Import data from a CSV file (*.csv)",
            IconKey = "file-text"
        };
    }

    public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
    {
        var datasource = new CsvDatasourceArtifact(definition.Name)
        {
            Description = definition.Description
        };

        // Parse settings
        if (definition.Settings.TryGetValue("FilePath", out var filePath))
            datasource.FilePath = filePath?.ToString() ?? string.Empty;

        if (definition.Settings.TryGetValue("FirstRowIsHeader", out var firstRowIsHeader) &&
            bool.TryParse(firstRowIsHeader?.ToString(), out var firstRowIsHeaderValue))
            datasource.FirstRowIsHeader = firstRowIsHeaderValue;

        if (definition.Settings.TryGetValue("FieldDelimiter", out var fieldDelimiter))
            datasource.FieldDelimiter = fieldDelimiter?.ToString() ?? ",";

        if (definition.Settings.TryGetValue("RowTerminator", out var rowTerminator))
            datasource.RowTerminator = rowTerminator?.ToString() ?? "\\n";

        return datasource;
    }

    public DatasourceArtifact CreateNew(string name)
    {
        return new CsvDatasourceArtifact(name);
    }

    public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
    {
        if (datasource is not CsvDatasourceArtifact csvDatasource)
            throw new ArgumentException("Datasource is not a CSV datasource", nameof(datasource));

        return new DatasourceDefinition
        {
            Id = datasource.Id,
            Name = datasource.Name,
            Type = TypeId,
            Description = datasource.Description,
            Settings = new Dictionary<string, object?>
            {
                ["FilePath"] = csvDatasource.FilePath,
                ["FirstRowIsHeader"] = csvDatasource.FirstRowIsHeader,
                ["FieldDelimiter"] = csvDatasource.FieldDelimiter,
                ["RowTerminator"] = csvDatasource.RowTerminator
            }
        };
    }

    public IEnumerable<GenericDataType> GetSupportedColumnDataTypes()
    {
        // CSV doesn't have a fixed set of data types - return common types
        return new List<GenericDataType>
        {
            GenericDataTypes.Text,
            GenericDataTypes.VarChar,
            GenericDataTypes.Int,
            GenericDataTypes.BigInt,
            GenericDataTypes.Decimal,
            GenericDataTypes.Double,
            GenericDataTypes.Boolean,
            GenericDataTypes.DateTime
        };
    }
}
