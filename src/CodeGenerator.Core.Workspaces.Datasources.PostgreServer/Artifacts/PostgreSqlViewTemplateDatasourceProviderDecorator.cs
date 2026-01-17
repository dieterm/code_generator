using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Dynamic;

namespace CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Artifacts;

public class PostgreSqlViewTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
{
    public PostgreSqlViewTemplateDatasourceProviderDecorator(ArtifactDecoratorState state)
        : base(state)
    {
    }

    public PostgreSqlViewTemplateDatasourceProviderDecorator(string key)
        : base(key)
    {
    }

    public override void Attach(Artifact artifact)
    {
        if (artifact is not ViewArtifact)
        {
            throw new InvalidOperationException("PostgreSqlViewTemplateDatasourceProviderDecorator can only be attached to ViewArtifact.");
        }
        base.Attach(artifact);
    }

    public override string DisplayName
    {
        get
        {
            var viewArtifact = this.Artifact as ViewArtifact;
            var schemaPrefix = string.IsNullOrEmpty(viewArtifact?.Schema) ? "" : $"{viewArtifact.Schema}.";

            if (viewArtifact?.Parent is RelationalDatabaseDatasourceArtifact datasource)
            {
                return $"{datasource.Name} > {schemaPrefix}{viewArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource name for PostgreSQL template datasource provider.");
        }
    }

    public override string FullPath
    {
        get
        {
            var viewArtifact = this.Artifact as ViewArtifact;
            var schemaPrefix = string.IsNullOrEmpty(viewArtifact?.Schema) ? "" : $"{viewArtifact.Schema}.";
            if (viewArtifact?.Parent is RelationalDatabaseDatasourceArtifact datasource)
            {
                return $"{datasource.Name}.{schemaPrefix}{viewArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource full path for PostgreSQL template datasource provider.");
        }
    }

    public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
    {
        var viewArtifact = this.Artifact as ViewArtifact;
        var results = new List<dynamic>();

        if (viewArtifact?.Parent is not RelationalDatabaseDatasourceArtifact datasource)
        {
            Logger.LogWarning("ViewArtifactData is only supported for PostgreSQL datasources");
            return results;
        }

        var postgreSqlDatabase = datasource.GetDomainRelationalDatabase();
        if (postgreSqlDatabase == null)
            throw new InvalidOperationException("Failed to get PostgreSQL database from datasource.");

        var columnNames = viewArtifact.GetColumns().Select(c => c.Name).ToList();
        // Build SELECT query
        var query = postgreSqlDatabase.GenerateSelectStatement(
            tableName: viewArtifact.Name,
            columnNames: columnNames,
            schema: viewArtifact.Schema,
            whereClause: filter,
            limit: maxRows);

        Logger.LogDebug("Executing query: {Query}", query);

        try
        {
            await using var connection = new NpgsqlConnection(datasource.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new ExpandoObject() as IDictionary<string, object?>;

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[columnName] = value;
                }

                results.Add(row);
            }

            Logger.LogInformation("Loaded {RowCount} rows from view {ViewName}", results.Count, viewArtifact.Name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading data from view {ViewName}", viewArtifact.Name);
            throw;
        }

        return results;
    }
}
