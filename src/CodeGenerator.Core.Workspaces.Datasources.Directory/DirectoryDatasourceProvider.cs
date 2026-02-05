using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Directory.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.Directory;

/// <summary>
/// Provider for Directory datasources
/// </summary>
public class DirectoryDatasourceProvider : IDatasourceProvider
{
    public string TypeId => DirectoryDatasourceArtifact.TYPE_ID;

    public DatasourceTypeInfo GetTypeInfo()
    {
        return new DatasourceTypeInfo
        {
            TypeId = TypeId,
            DisplayName = "Directory",
            Category = "File System",
            Description = "Import file and folder structure from a directory",
            IconKey = "folder-open"
        };
    }

    public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
    {
        if (datasource is not DirectoryDatasourceArtifact directoryDatasource)
            throw new ArgumentException("Datasource is not a Directory datasource", nameof(datasource));

        return new DatasourceDefinition
        {
            Id = datasource.Id,
            Name = datasource.Name,
            Type = TypeId,
            Description = datasource.Description,
            Settings = new Dictionary<string, object?>
            {
                ["DirectoryPath"] = directoryDatasource.DirectoryPath,
                ["SearchPattern"] = directoryDatasource.SearchPattern,
                ["IncludeSubdirectories"] = directoryDatasource.IncludeSubdirectories
            }
        };
    }

    public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
    {
        var datasource = new DirectoryDatasourceArtifact(definition.Name)
        {
            Description = definition.Description
        };

        // Parse settings
        if (definition.Settings.TryGetValue("DirectoryPath", out var directoryPath))
            datasource.DirectoryPath = directoryPath?.ToString() ?? string.Empty;

        if (definition.Settings.TryGetValue("SearchPattern", out var searchPattern))
            datasource.SearchPattern = searchPattern?.ToString() ?? "*.*";

        if (definition.Settings.TryGetValue("IncludeSubdirectories", out var includeSubdirs))
            datasource.IncludeSubdirectories = includeSubdirs is bool b && b;

        return datasource;
    }

    public DatasourceArtifact CreateNew(string name)
    {
        return new DirectoryDatasourceArtifact(name);
    }

    public IEnumerable<GenericDataType> GetSupportedColumnDataTypes()
    {
        return new List<GenericDataType>
        {
            GenericDataTypes.VarChar,
            GenericDataTypes.Int,
            GenericDataTypes.BigInt,
            GenericDataTypes.Boolean,
            GenericDataTypes.DateTime,
            GenericDataTypes.Json
        };
    }
}
