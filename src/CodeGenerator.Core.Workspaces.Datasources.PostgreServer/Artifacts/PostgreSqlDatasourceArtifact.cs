using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.Databases.RelationalDatabases;
using Npgsql;

namespace CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Artifacts;

/// <summary>
/// Represents a PostgreSQL database datasource
/// </summary>
public class PostgreSqlDatasourceArtifact : RelationalDatabaseDatasourceArtifact
{
    public const string TYPE_ID = "PostgreSql";
    public const string PostgreSqlTemplateDatasourceProviderDecorator_ID = "PostgreSqlTemplateDatasourceProviderDecorator";

    public PostgreSqlDatasourceArtifact(string name, string? description = null)
        : base(name, description)
    {
        Server = "localhost";
        Port = 5432;
        Database = string.Empty;
        Username = "postgres";
        Password = string.Empty;
        SslMode = "Prefer";
    }

    public PostgreSqlDatasourceArtifact(ArtifactState state)
        : base(state)
    {
    }

    public override TemplateDatasourceProviderDecorator CreateTableArtifactTemplateDatasourceProviderDecorator()
    {
        return new PostgreSqlTableTemplateDatasourceProviderDecorator(PostgreSqlTemplateDatasourceProviderDecorator_ID);
    }

    public override TemplateDatasourceProviderDecorator CreateViewArtifactTemplateDatasourceProviderDecorator()
    {
        return new PostgreSqlViewTemplateDatasourceProviderDecorator(PostgreSqlTemplateDatasourceProviderDecorator_ID);
    }

    public override T AddChild<T>(T child)
    {
        base.AddChild(child);
        if (child is TableArtifact tableArtifact)
        {
            // first check if it already has the decorator (from memento state restore)
            if (tableArtifact.GetTemplateDatasourceProviderDecorator() != null)
                return child;
            tableArtifact.AddDecorator(CreateTableArtifactTemplateDatasourceProviderDecorator());
        }
        else if (child is ViewArtifact viewArtifact)
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
    /// Port number (default: 5432)
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

    /// <summary>
    /// SSL Mode (Disable, Allow, Prefer, Require, VerifyCA, VerifyFull)
    /// </summary>
    public string SslMode
    {
        get => GetValue<string>(nameof(SslMode));
        set
        {
            if (SetValue(nameof(SslMode), value))
                UpdateConnectionString();
        }
    }

    private void UpdateConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Server ?? "localhost",
            Port = Port > 0 ? Port : 5432,
            Database = Database ?? string.Empty,
            Username = Username ?? string.Empty,
            Password = Password ?? string.Empty
        };

        // Set SSL mode
        if (!string.IsNullOrEmpty(SslMode) && Enum.TryParse<SslMode>(SslMode, true, out var sslModeValue))
        {
            builder.SslMode = sslModeValue;
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
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            Server = builder.Host ?? "localhost";
            Port = builder.Port;
            Database = builder.Database ?? string.Empty;
            Username = builder.Username ?? string.Empty;
            Password = builder.Password ?? string.Empty;
            SslMode = builder.SslMode.ToString();
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
            await using var connection = new NpgsqlConnection(ConnectionString);
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
        return PostgreSqlDatabase.Instance;
    }
}
