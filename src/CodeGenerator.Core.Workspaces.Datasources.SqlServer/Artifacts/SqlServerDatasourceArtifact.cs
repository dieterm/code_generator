using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.Databases.RelationalDatabases;
using Microsoft.Data.SqlClient;

namespace CodeGenerator.Core.Workspaces.Datasources.SqlServer.Artifacts;

/// <summary>
/// Represents a SQL Server database datasource
/// </summary>
public class SqlServerDatasourceArtifact : RelationalDatabaseDatasourceArtifact
{
    public const string TYPE_ID = "SqlServer";
    public const string SqlServerTemplateDatasourceProviderDecorator_ID = "SqlServerTemplateDatasourceProviderDecorator";

    public SqlServerDatasourceArtifact(string name, string? description = null)
        : base(name, description)
    {
        Server = "localhost";
        Database = string.Empty;
        IntegratedSecurity = true;
        Username = string.Empty;
        Password = string.Empty;
        TrustServerCertificate = true;
    }

    public SqlServerDatasourceArtifact(ArtifactState state)
        : base(state)
    {
    }

    public override TemplateDatasourceProviderDecorator CreateTableArtifactTemplateDatasourceProviderDecorator()
    {
        return new SqlServerTableTemplateDatasourceProviderDecorator(SqlServerTemplateDatasourceProviderDecorator_ID);
    }

    public override TemplateDatasourceProviderDecorator CreateViewArtifactTemplateDatasourceProviderDecorator()
    {
        return new SqlServerViewTemplateDatasourceProviderDecorator(SqlServerTemplateDatasourceProviderDecorator_ID);
    }

    public override void AddChild(IArtifact child)
    {
        base.AddChild(child);
        if (child is TableArtifact tableArtifact)
        {
            // first check if it already has the decorator (from memento state restore)
            if (tableArtifact.GetTemplateDatasourceProviderDecorator() != null)
                return;
            tableArtifact.AddDecorator(CreateTableArtifactTemplateDatasourceProviderDecorator());
        }
        else if (child is ViewArtifact viewArtifact)
        {
            // first check if it already has the decorator (from memento state restore)
            if (viewArtifact.GetTemplateDatasourceProviderDecorator() != null)
                return;
            viewArtifact.AddDecorator(CreateViewArtifactTemplateDatasourceProviderDecorator());
        }
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
    /// Server hostname or IP address (can include instance name, e.g., "server\instance")
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
    /// Use Windows Authentication (Integrated Security)
    /// </summary>
    public bool IntegratedSecurity
    {
        get => GetValue<bool>(nameof(IntegratedSecurity));
        set
        {
            if (SetValue(nameof(IntegratedSecurity), value))
                UpdateConnectionString();
        }
    }

    /// <summary>
    /// Username for SQL Server authentication
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
    /// Password for SQL Server authentication
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

    /// <summary>
    /// Trust server certificate (useful for development environments)
    /// </summary>
    public bool TrustServerCertificate
    {
        get => GetValue<bool>(nameof(TrustServerCertificate));
        set
        {
            if (SetValue(nameof(TrustServerCertificate), value))
                UpdateConnectionString();
        }
    }

    private void UpdateConnectionString()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = Server ?? "localhost",
            InitialCatalog = Database ?? string.Empty,
            IntegratedSecurity = IntegratedSecurity,
            TrustServerCertificate = TrustServerCertificate
        };

        if (!IntegratedSecurity)
        {
            builder.UserID = Username ?? string.Empty;
            builder.Password = Password ?? string.Empty;
        }

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
            var builder = new SqlConnectionStringBuilder(connectionString);
            Server = builder.DataSource;
            Database = builder.InitialCatalog;
            IntegratedSecurity = builder.IntegratedSecurity;
            Username = builder.UserID;
            Password = builder.Password;
            TrustServerCertificate = builder.TrustServerCertificate;
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
            await using var connection = new SqlConnection(ConnectionString);
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
        return SqlServerDatabase.Instance;
    }
}
