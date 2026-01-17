using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Dynamic;

namespace CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Artifacts;

public class PostgreSqlTableTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
{
    public PostgreSqlTableTemplateDatasourceProviderDecorator(ArtifactDecoratorState state)
        : base(state)
    {
    }

    public PostgreSqlTableTemplateDatasourceProviderDecorator(string key)
        : base(key)
    {
    }

    public override void Attach(Artifact artifact)
    {
        if (artifact is not TableArtifact)
        {
            throw new InvalidOperationException("PostgreSqlTableTemplateDatasourceProviderDecorator can only be attached to TableArtifact.");
        }
        base.Attach(artifact);
    }

    public override string DisplayName
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;
            var schemaPrefix = string.IsNullOrEmpty(tableArtifact?.Schema) ? "" : $"{tableArtifact.Schema}.";

            if (tableArtifact?.Parent is RelationalDatabaseDatasourceArtifact datasource)
            {
                return $"{datasource.Name} > {schemaPrefix}{tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource name for PostgreSQL template datasource provider.");
        }
    }

    public override string FullPath
    {
        get
        {
            var tableArtifact = this.Artifact as TableArtifact;
            var schemaPrefix = string.IsNullOrEmpty(tableArtifact?.Schema) ? "" : $"{tableArtifact.Schema}.";
            if (tableArtifact?.Parent is RelationalDatabaseDatasourceArtifact datasource)
            {
                return $"{datasource.Name}.{schemaPrefix}{tableArtifact.Name}";
            }
            throw new InvalidOperationException("Failed to determine datasource full path for PostgreSQL template datasource provider.");
        }
    }

    public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
    {
        var tableArtifact = this.Artifact as TableArtifact;
        var results = new List<dynamic>();

        if (tableArtifact?.Parent is not RelationalDatabaseDatasourceArtifact datasource)
        {
            Logger.LogWarning("TableArtifactData is only supported for PostgreSQL datasources");
            return results;
        }

        var postgreSqlDatabase = datasource.GetDomainRelationalDatabase();
        if (postgreSqlDatabase == null)
            throw new InvalidOperationException("Failed to get PostgreSQL database from datasource.");

        var columnNames = tableArtifact.GetColumns().Select(c => c.Name).ToList();
        // Build SELECT query
        var query = postgreSqlDatabase.GenerateSelectStatement(
            tableName: tableArtifact.Name,
            columnNames: columnNames,
            schema: tableArtifact.Schema,
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

            Logger.LogInformation("Loaded {RowCount} rows from table {TableName}", results.Count, tableArtifact.Name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading data from table {TableName}", tableArtifact.Name);
            throw;
        }

        return results;
    }
}
