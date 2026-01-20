using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Xml.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.Xml;

/// <summary>
/// Provider for XML file datasources
/// </summary>
public class XmlDatasourceProvider : IDatasourceProvider
{
    public string TypeId => XmlDatasourceArtifact.TYPE_ID;

    public DatasourceTypeInfo GetTypeInfo()
    {
        return new DatasourceTypeInfo
        {
            TypeId = TypeId,
            DisplayName = "XML File",
            Category = "File",
            Description = "Import data from an XML file (*.xml)",
            IconKey = "file-code"
        };
    }

    public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
    {
        var datasource = new XmlDatasourceArtifact(definition.Name)
        {
            Description = definition.Description
        };

        // Parse settings
        if (definition.Settings.TryGetValue("FilePath", out var filePath))
            datasource.FilePath = filePath?.ToString() ?? string.Empty;

        if (definition.Settings.TryGetValue("RootElementPath", out var rootPath))
            datasource.RootElementPath = rootPath?.ToString();

        return datasource;
    }

    public DatasourceArtifact CreateNew(string name)
    {
        return new XmlDatasourceArtifact(name);
    }

    public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
    {
        if (datasource is not XmlDatasourceArtifact xmlDatasource)
            throw new ArgumentException("Datasource is not an XML datasource", nameof(datasource));

        return new DatasourceDefinition
        {
            Id = datasource.Id,
            Name = datasource.Name,
            Type = TypeId,
            Description = datasource.Description,
            Settings = new Dictionary<string, object?>
            {
                ["FilePath"] = xmlDatasource.FilePath,
                ["RootElementPath"] = xmlDatasource.RootElementPath
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
        // XML supports various data types
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
            GenericDataTypes.Xml
        };
    }
}
