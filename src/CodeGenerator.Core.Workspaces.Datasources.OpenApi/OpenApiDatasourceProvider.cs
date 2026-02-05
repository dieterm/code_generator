using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.OpenApi.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.OpenApi;

/// <summary>
/// Provider for OpenAPI/Swagger datasources
/// </summary>
public class OpenApiDatasourceProvider : IDatasourceProvider
{
    public string TypeId => OpenApiDatasourceArtifact.TYPE_ID;

    public DatasourceTypeInfo GetTypeInfo()
    {
        return new DatasourceTypeInfo
        {
            TypeId = TypeId,
            DisplayName = "OpenAPI/Swagger",
            Category = "File",
            Description = "Import schemas from an OpenAPI/Swagger specification file (*.yaml, *.json)",
            IconKey = "file-code"
        };
    }

    public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
    {
        var datasource = new OpenApiDatasourceArtifact(definition.Name)
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
        return new OpenApiDatasourceArtifact(name);
    }

    public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
    {
        if (datasource is not OpenApiDatasourceArtifact openApiDatasource)
            throw new ArgumentException("Datasource is not an OpenAPI datasource", nameof(datasource));

        return new DatasourceDefinition
        {
            Id = datasource.Id,
            Name = datasource.Name,
            Type = TypeId,
            Description = datasource.Description,
            Settings = new Dictionary<string, object?>
            {
                ["FilePath"] = openApiDatasource.FilePath
            }
        };
    }

    public IEnumerable<GenericDataType> GetSupportedColumnDataTypes()
    {
        // OpenAPI supports various data types
        return new List<GenericDataType>
        {
            GenericDataTypes.VarChar,
            GenericDataTypes.Int,
            GenericDataTypes.BigInt,
            GenericDataTypes.Decimal,
            GenericDataTypes.Double,
            GenericDataTypes.Float,
            GenericDataTypes.Boolean,
            GenericDataTypes.DateTime,
            GenericDataTypes.Date,
            GenericDataTypes.Guid,
            GenericDataTypes.Binary,
            GenericDataTypes.Json
        };
    }
}
