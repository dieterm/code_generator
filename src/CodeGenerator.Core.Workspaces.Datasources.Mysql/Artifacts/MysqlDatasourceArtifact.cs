using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.Databases.RelationalDatabases;
using MySqlConnector;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.Artifacts
{
    /// <summary>
    /// Represents a MySQL/MariaDB database datasource
    /// </summary>
    public class MysqlDatasourceArtifact : RelationalDatabaseDatasourceArtifact
    {
        public const string TYPE_ID = "MySql";
        public const string MysqlTemplateDatasourceProviderDecorator_ID = "MysqlTemplateDatasourceProviderDecorator";
        public MysqlDatasourceArtifact(string name, string? description = null) 
            : base(name, description)
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

        public override TemplateDatasourceProviderDecorator CreateTableArtifactTemplateDatasourceProviderDecorator()
        {
            return new MysqlTableTemplateDatasourceProviderDecorator(MysqlTemplateDatasourceProviderDecorator_ID);
        }
        public override TemplateDatasourceProviderDecorator CreateViewArtifactTemplateDatasourceProviderDecorator()
        {
            return new MysqlViewTemplateDatasourceProviderDecorator(MysqlTemplateDatasourceProviderDecorator_ID);
        }

        public override T AddChild<T>(T child)
        {
            base.AddChild(child);
            if(child is TableArtifact tableArtifact)
            {
                // first check if it already has the decorator (from memento state restore)
                if (tableArtifact.GetTemplateDatasourceProviderDecorator() != null)
                    return child;
                tableArtifact.AddDecorator(CreateTableArtifactTemplateDatasourceProviderDecorator());
            }
            else if(child is ViewArtifact viewArtifact)
            {
                // first check if it already has the decorator (from memento state restore)
                if (viewArtifact.GetTemplateDatasourceProviderDecorator() != null)
                    return child;
                viewArtifact.AddDecorator(CreateViewArtifactTemplateDatasourceProviderDecorator());
            }
            return child;
        }

        public override void RemoveChild(IArtifact child)
        {
            base.RemoveChild(child);
            if (child is TableArtifact tableArtifact)
            {
                var decorator = tableArtifact.GetTemplateDatasourceProviderDecorator();
                if (decorator != null)
                {
                    tableArtifact.RemoveDecorator(decorator);
                }
            }
            else if (child is ViewArtifact viewArtifact)
            {
                var decorator = viewArtifact.GetTemplateDatasourceProviderDecorator();
                if (decorator != null)
                {
                    viewArtifact.RemoveDecorator(decorator);
                }
            }
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

        public override bool CanBeginEdit()
        {
            return true;
        }

        public override bool Validating(string newName)
        {
            return !string.IsNullOrWhiteSpace(newName);
        }

        public override void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }

        public override RelationalDatabase GetDomainRelationalDatabase()
        {
            return MysqlDatabase.Instance;
        }

 
    }
}
