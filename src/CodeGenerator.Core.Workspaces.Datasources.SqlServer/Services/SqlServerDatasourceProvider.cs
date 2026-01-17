using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.SqlServer.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Core.Workspaces.Datasources.SqlServer.Services;

/// <summary>
/// Provider for SQL Server datasources
/// </summary>
public class SqlServerDatasourceProvider : IDatasourceProvider
{
    public string TypeId => SqlServerDatasourceArtifact.TYPE_ID;

    public DatasourceTypeInfo GetTypeInfo()
    {
        return new DatasourceTypeInfo
        {
            TypeId = TypeId,
            DisplayName = "SQL Server",
            Category = "Relational Database",
            Description = "Connect to a Microsoft SQL Server database",
            IconKey = "database"
        };
    }

    public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
    {
        var datasource = new SqlServerDatasourceArtifact(definition.Name)
        {
            Description = definition.Description
        };

        // Parse settings
        if (definition.Settings.TryGetValue("Server", out var server))
            datasource.Server = server?.ToString() ?? "localhost";

        if (definition.Settings.TryGetValue("Database", out var database))
            datasource.Database = database?.ToString() ?? string.Empty;

        if (definition.Settings.TryGetValue("IntegratedSecurity", out var integratedSecurity) && 
            bool.TryParse(integratedSecurity?.ToString(), out var integratedSecurityValue))
            datasource.IntegratedSecurity = integratedSecurityValue;

        if (definition.Settings.TryGetValue("Username", out var username))
            datasource.Username = username?.ToString() ?? string.Empty;

        if (definition.Settings.TryGetValue("Password", out var password))
            datasource.Password = password?.ToString() ?? string.Empty;

        if (definition.Settings.TryGetValue("TrustServerCertificate", out var trustCert) &&
            bool.TryParse(trustCert?.ToString(), out var trustCertValue))
            datasource.TrustServerCertificate = trustCertValue;

        if (definition.Settings.TryGetValue("ConnectionString", out var connectionString))
            datasource.ParseConnectionString(connectionString?.ToString() ?? string.Empty);

        return datasource;
    }

    public DatasourceArtifact CreateNew(string name)
    {
        return new SqlServerDatasourceArtifact(name);
    }

    public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
    {
        if (datasource is not SqlServerDatasourceArtifact sqlServerDatasource)
            throw new ArgumentException("Datasource is not a SQL Server datasource", nameof(datasource));

        return new DatasourceDefinition
        {
            Id = datasource.Id,
            Name = datasource.Name,
            Type = TypeId,
            Description = datasource.Description,
            Settings = new Dictionary<string, object?>
            {
                ["Server"] = sqlServerDatasource.Server,
                ["Database"] = sqlServerDatasource.Database,
                ["IntegratedSecurity"] = sqlServerDatasource.IntegratedSecurity,
                ["Username"] = sqlServerDatasource.Username,
                ["Password"] = sqlServerDatasource.Password,
                ["TrustServerCertificate"] = sqlServerDatasource.TrustServerCertificate,
                ["ConnectionString"] = sqlServerDatasource.ConnectionString
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
        return Domain.Databases.RelationalDatabases.SqlServerDatabase.Instance.DataTypeMappings
            .Select(m => m.GenericType).Distinct();
    }
}
