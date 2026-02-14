using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.DataTypes;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.Services
{
    /// <summary>
    /// Provider for MySQL/MariaDB datasources
    /// </summary>
    public class MysqlDatasourceProvider : IDatasourceProvider
    {
        public string TypeId => MysqlDatasourceArtifact.TYPE_ID;

        public DatasourceTypeInfo GetTypeInfo()
        {
            return new DatasourceTypeInfo
            {
                TypeId = TypeId,
                DisplayName = "MySQL / MariaDB",
                Category = DatasourceCategory.RelationalDatabase,
                Description = "Connect to a MySQL or MariaDB database",
                IconKey = "database"
            };
        }

        public DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition)
        {
            var datasource = new MysqlDatasourceArtifact(definition.Name)
            {
                Description = definition.Description
            };

            // Parse settings
            if (definition.Settings.TryGetValue("Server", out var server))
                datasource.Server = server?.ToString() ?? "localhost";

            if (definition.Settings.TryGetValue("Port", out var port) && int.TryParse(port?.ToString(), out var portValue))
                datasource.Port = portValue;

            if (definition.Settings.TryGetValue("Database", out var database))
                datasource.Database = database?.ToString() ?? string.Empty;

            if (definition.Settings.TryGetValue("Username", out var username))
                datasource.Username = username?.ToString() ?? string.Empty;

            if (definition.Settings.TryGetValue("Password", out var password))
                datasource.Password = password?.ToString() ?? string.Empty;

            if (definition.Settings.TryGetValue("ConnectionString", out var connectionString))
                datasource.ParseConnectionString(connectionString?.ToString() ?? string.Empty);

            return datasource;
        }

        public DatasourceArtifact CreateNew(string name)
        {
            return new MysqlDatasourceArtifact(name);
        }

        public DatasourceDefinition CreateDefinition(DatasourceArtifact datasource)
        {
            if (datasource is not MysqlDatasourceArtifact mysqlDatasource)
                throw new ArgumentException("Datasource is not a MySQL datasource", nameof(datasource));

            return new DatasourceDefinition
            {
                Id = datasource.Id,
                Name = datasource.Name,
                Type = TypeId,
                Description = datasource.Description,
                Settings = new Dictionary<string, object?>
                {
                    ["Server"] = mysqlDatasource.Server,
                    ["Port"] = mysqlDatasource.Port,
                    ["Database"] = mysqlDatasource.Database,
                    ["Username"] = mysqlDatasource.Username,
                    ["Password"] = mysqlDatasource.Password,
                    ["ConnectionString"] = mysqlDatasource.ConnectionString
                }
            };
        }

        public IEnumerable<GenericDataType> GetSupportedColumnDataTypes()
        {
            return Domain.Databases.RelationalDatabases.MysqlDatabase.Instance.DataTypeMappings.Select(m => m.GenericType).Distinct();
        }
    }
}
