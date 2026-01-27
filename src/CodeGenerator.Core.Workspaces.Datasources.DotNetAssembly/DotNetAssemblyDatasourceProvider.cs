using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly;

/// <summary>
/// Provider for .NET Assembly datasources
/// </summary>
public class DotNetAssemblyDatasourceProvider : IDatasourceProvider
{
    public string TypeId => DotNetAssemblyDatasourceArtifact.TYPE_ID;

    public DatasourceTypeInfo GetTypeInfo()
    {
        return new DatasourceTypeInfo
        {
            TypeId = TypeId,
            DisplayName = ".NET Assembly",
            Category = "File",
            Description = "Import types from a .NET assembly (*.dll, *.exe)",
            IconKey = "file-cog"
        };
    }

    public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
    {
        var datasource = new DotNetAssemblyDatasourceArtifact(definition.Name)
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
        return new DotNetAssemblyDatasourceArtifact(name);
    }

    public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
    {
        if (datasource is not DotNetAssemblyDatasourceArtifact assemblyDatasource)
            throw new ArgumentException("Datasource is not a .NET Assembly datasource", nameof(datasource));

        return new DatasourceDefinition
        {
            Id = datasource.Id,
            Name = datasource.Name,
            Type = TypeId,
            Description = datasource.Description,
            Settings = new Dictionary<string, object?>
            {
                ["FilePath"] = assemblyDatasource.FilePath
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
        // .NET types map to various generic data types
        return new List<GenericDataType>
        {
            GenericDataTypes.VarChar,
            GenericDataTypes.Int,
            GenericDataTypes.BigInt,
            GenericDataTypes.SmallInt,
            GenericDataTypes.TinyInt,
            GenericDataTypes.Decimal,
            GenericDataTypes.Double,
            GenericDataTypes.Float,
            GenericDataTypes.Boolean,
            GenericDataTypes.DateTime,
            GenericDataTypes.DateTimeOffset,
            GenericDataTypes.Date,
            GenericDataTypes.Time,
            GenericDataTypes.Guid,
            GenericDataTypes.Binary,
            GenericDataTypes.Json
        };
    }
}
