using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Excel.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.Excel;

/// <summary>
/// Provider for Excel file datasources
/// </summary>
public class ExcelDatasourceProvider : IDatasourceProvider
{
    public string TypeId => ExcelDatasourceArtifact.TYPE_ID;

    public DatasourceTypeInfo GetTypeInfo()
    {
        return new DatasourceTypeInfo
        {
            TypeId = TypeId,
            DisplayName = "Excel File",
            Category = "File",
            Description = "Import data from an Excel file (*.xlsx, *.xls)",
            IconKey = "file-spreadsheet"
        };
    }

    public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
    {
        var datasource = new ExcelDatasourceArtifact(definition.Name)
        {
            Description = definition.Description
        };

        // Parse settings
        if (definition.Settings.TryGetValue("FilePath", out var filePath))
            datasource.FilePath = filePath?.ToString() ?? string.Empty;

        if (definition.Settings.TryGetValue("FirstRowIsHeader", out var firstRowIsHeader) &&
            bool.TryParse(firstRowIsHeader?.ToString(), out var firstRowIsHeaderValue))
            datasource.FirstRowIsHeader = firstRowIsHeaderValue;

        return datasource;
    }

    public DatasourceArtifact CreateNew(string name)
    {
        return new ExcelDatasourceArtifact(name);
    }

    public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
    {
        if (datasource is not ExcelDatasourceArtifact excelDatasource)
            throw new ArgumentException("Datasource is not an Excel datasource", nameof(datasource));

        return new DatasourceDefinition
        {
            Id = datasource.Id,
            Name = datasource.Name,
            Type = TypeId,
            Description = datasource.Description,
            Settings = new Dictionary<string, object?>
            {
                ["FilePath"] = excelDatasource.FilePath,
                ["FirstRowIsHeader"] = excelDatasource.FirstRowIsHeader
            }
        };
    }

    public IEnumerable<GenericDataType> GetSupportedColumnDataTypes()
    {
        // Excel doesn't have a fixed set of data types - return common types
        return new List<GenericDataType>
        {
            GenericDataTypes.Text,
            GenericDataTypes.Int,
            GenericDataTypes.BigInt,
            GenericDataTypes.Decimal,
            GenericDataTypes.Double,
            GenericDataTypes.Boolean,
            GenericDataTypes.DateTime
        };
    }
}
