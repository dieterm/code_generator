using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.PostgreSql;

/// <summary>
/// Provider for PostgreSQL datasources
/// </summary>
public class PostgreSqlDatasourceProvider : IDatasourceProvider
{
    public string TypeId => PostgreSqlDatasourceArtifact.TYPE_ID;

    public DatasourceTypeInfo GetTypeInfo()
    {
        return new DatasourceTypeInfo
        {
            TypeId = TypeId,
            DisplayName = "PostgreSQL",
            Category = DatasourceCategory.RelationalDatabase,
            Description = "Connect to a PostgreSQL database",
            IconKey = "database"
        };
    }

    public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
    {
        var datasource = new PostgreSqlDatasourceArtifact(definition.Name)
        {
            Description = definition.Description
        };

        // Parse settings
        if (definition.Settings.TryGetValue("Server", out var server))
            datasource.Server = server?.ToString() ?? "localhost";

        if (definition.Settings.TryGetValue("Port", out var port) &&
            int.TryParse(port?.ToString(), out var portValue))
            datasource.Port = portValue;

        if (definition.Settings.TryGetValue("Database", out var database))
            datasource.Database = database?.ToString() ?? string.Empty;

        if (definition.Settings.TryGetValue("Username", out var username))
            datasource.Username = username?.ToString() ?? "postgres";

        if (definition.Settings.TryGetValue("Password", out var password))
            datasource.Password = password?.ToString() ?? string.Empty;

        if (definition.Settings.TryGetValue("SslMode", out var sslMode))
            datasource.SslMode = sslMode?.ToString() ?? "Prefer";

        if (definition.Settings.TryGetValue("ConnectionString", out var connectionString))
            datasource.ParseConnectionString(connectionString?.ToString() ?? string.Empty);

        return datasource;
    }

    public DatasourceArtifact CreateNew(string name)
    {
        return new PostgreSqlDatasourceArtifact(name);
    }

    public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
    {
        if (datasource is not PostgreSqlDatasourceArtifact postgreSqlDatasource)
            throw new ArgumentException("Datasource is not a PostgreSQL datasource", nameof(datasource));

        return new DatasourceDefinition
        {
            Id = datasource.Id,
            Name = datasource.Name,
            Type = TypeId,
            Description = datasource.Description,
            Settings = new Dictionary<string, object?>
            {
                ["Server"] = postgreSqlDatasource.Server,
                ["Port"] = postgreSqlDatasource.Port,
                ["Database"] = postgreSqlDatasource.Database,
                ["Username"] = postgreSqlDatasource.Username,
                ["Password"] = postgreSqlDatasource.Password,
                ["SslMode"] = postgreSqlDatasource.SslMode,
                ["ConnectionString"] = postgreSqlDatasource.ConnectionString
            }
        };
    }

    public IEnumerable<GenericDataType> GetSupportedColumnDataTypes()
    {
        return Domain.Databases.RelationalDatabases.PostgreSqlDatabase.Instance.DataTypeMappings
            .Select(m => m.GenericType).Distinct();
    }
}
