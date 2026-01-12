using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using MySqlConnector;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.Artifacts
{
    /// <summary>
    /// Represents a MySQL/MariaDB database datasource
    /// </summary>
    public class MysqlDatasourceArtifact : RelationalDatabaseDatasourceArtifact
    {
        public const string TYPE_ID = "MySql";

        public MysqlDatasourceArtifact(string name) 
            : base(name)
        {
            Server = "localhost";
            Port = 3306;
            Database = string.Empty;
            Username = "root";
            Password = string.Empty;
        }

        public MysqlDatasourceArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string DatasourceType => TYPE_ID;

        protected override string IconKey => "database";

        /// <summary>
        /// Server hostname or IP address
        /// </summary>
        public string Server
        {
            get => GetValue<string>(nameof(Server));
            set
            {
                if (SetValue(nameof(Server), value))
                    UpdateConnectionString();
            }
        }

        /// <summary>
        /// Server port (default: 3306)
        /// </summary>
        public int Port
        {
            get => GetValue<int>(nameof(Port));
            set
            {
                if (SetValue(nameof(Port), value))
                    UpdateConnectionString();
            }
        }

        /// <summary>
        /// Database name
        /// </summary>
        public string Database
        {
            get => GetValue<string>(nameof(Database));
            set
            {
                if (SetValue(nameof(Database), value))
                    UpdateConnectionString();
            }
        }

        /// <summary>
        /// Username for authentication
        /// </summary>
        public string Username
        {
            get => GetValue<string>(nameof(Username));
            set
            {
                if (SetValue(nameof(Username), value))
                    UpdateConnectionString();
            }
        }

        /// <summary>
        /// Password for authentication
        /// </summary>
        public string Password
        {
            get => GetValue<string>(nameof(Password));
            set
            {
                if (SetValue(nameof(Password), value))
                    UpdateConnectionString();
            }
        }

        private void UpdateConnectionString()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = Server ?? "localhost",
                Port = (uint)(Port > 0 ? Port : 3306),
                Database = Database ?? string.Empty,
                UserID = Username ?? string.Empty,
                Password = Password ?? string.Empty
            };
            
            // Update base connection string without triggering recursion
            base.SetValue(nameof(ConnectionString), builder.ConnectionString);
        }

        /// <summary>
        /// Parse connection string and update individual properties
        /// </summary>
        public void ParseConnectionString(string connectionString)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                Server = builder.Server;
                Port = (int)builder.Port;
                Database = builder.Database;
                Username = builder.UserID;
                Password = builder.Password;
            }
            catch
            {
                // Invalid connection string, ignore
            }
        }

        public override async Task<bool> ValidateAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await using var connection = new MySqlConnection(ConnectionString);
                await connection.OpenAsync(cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task RefreshSchemaAsync(CancellationToken cancellationToken = default)
        {
            // Schema refresh is handled separately through the edit view
            await Task.CompletedTask;
        }
    }
}
