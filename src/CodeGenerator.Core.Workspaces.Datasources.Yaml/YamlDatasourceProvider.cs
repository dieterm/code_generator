using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Yaml.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.Yaml;

/// <summary>
/// Provider for YAML file datasources
/// </summary>
public class YamlDatasourceProvider : IDatasourceProvider
{
    public string TypeId => YamlDatasourceArtifact.TYPE_ID;

    public DatasourceTypeInfo GetTypeInfo()
    {
        return new DatasourceTypeInfo
        {
            TypeId = TypeId,
            DisplayName = "YAML File",
            Category = DatasourceCategory.File,
            Description = "Import data from a YAML file (*.yaml, *.yml)",
            IconKey = "file-code",
            FilePickerFilter = "YAML Files (*.yaml;*.yml)|*.yaml;*.yml"
        };
    }

    public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
    {
        var datasource = new YamlDatasourceArtifact(definition.Name)
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
        return new YamlDatasourceArtifact(name);
    }

    public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
    {
        if (datasource is not YamlDatasourceArtifact yamlDatasource)
            throw new ArgumentException("Datasource is not a YAML datasource", nameof(datasource));

        return new DatasourceDefinition
        {
            Id = datasource.Id,
            Name = datasource.Name,
            Type = TypeId,
            Description = datasource.Description,
            Settings = new Dictionary<string, object?>
            {
                ["FilePath"] = yamlDatasource.FilePath
            }
        };
    }

    public IEnumerable<GenericDataType> GetSupportedColumnDataTypes()
    {
        // YAML supports various data types
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
