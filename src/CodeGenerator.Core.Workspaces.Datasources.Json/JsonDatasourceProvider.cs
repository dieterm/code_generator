using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Json.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.Json;

/// <summary>
/// Provider for JSON file datasources
/// </summary>
public class JsonDatasourceProvider : IDatasourceProvider
{
    public string TypeId => JsonDatasourceArtifact.TYPE_ID;

    public DatasourceTypeInfo GetTypeInfo()
    {
        return new DatasourceTypeInfo
        {
            TypeId = TypeId,
            DisplayName = "JSON File",
            Category = "File",
            Description = "Import data from a JSON file (*.json)",
            IconKey = "file-json"
        };
    }

    public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
    {
        var datasource = new JsonDatasourceArtifact(definition.Name)
        {
            Description = definition.Description
        };

        // Parse settings
        if (definition.Settings.TryGetValue("FilePath", out var filePath))
            datasource.FilePath = filePath?.ToString() ?? string.Empty;

        return datasource;
    }

    public DatasourceArtifact CreateNew(string name)
    {
        return new JsonDatasourceArtifact(name);
    }

    public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
    {
        if (datasource is not JsonDatasourceArtifact jsonDatasource)
            throw new ArgumentException("Datasource is not a JSON datasource", nameof(datasource));

        return new DatasourceDefinition
        {
            Id = datasource.Id,
            Name = datasource.Name,
            Type = TypeId,
            Description = datasource.Description,
            Settings = new Dictionary<string, object?>
            {
                ["FilePath"] = jsonDatasource.FilePath
            }
        };
    }

    public Type? GetControllerType()
    {
        // Controller will be registered separately
        return null;
    }

    public IEnumerable<GenericDataType> GetSupportedColumnDataTypes()
    {
        // JSON supports various data types
        return new List<GenericDataType>
        {
            GenericDataTypes.VarChar,
            GenericDataTypes.Int,
            GenericDataTypes.BigInt,
            GenericDataTypes.Decimal,
            GenericDataTypes.Double,
            GenericDataTypes.Boolean,
            GenericDataTypes.DateTime,
            GenericDataTypes.Guid,
            GenericDataTypes.Json
        };
    }
}
