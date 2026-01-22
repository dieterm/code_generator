using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace CodeGenerator.Core.Workspaces.Datasources.SqlServer.Artifacts;

public class SqlServerTableTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
{
    public SqlServerTableTemplateDatasourceProviderDecorator(ArtifactDecoratorState state)
        : base(state)
    {
    }

    public SqlServerTableTemplateDatasourceProviderDecorator(string key)
        : base(key)
    {
    }

    public override void Attach(Artifact artifact)
    {
        if (artifact is not TableArtifact)
        {
            throw new InvalidOperationException("SqlServerTableTemplateDatasourceProviderDecorator can only be attached to TableArtifact.");
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
            throw new InvalidOperationException("Failed to determine datasource name for SQL Server template datasource provider.");
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
            throw new InvalidOperationException("Failed to determine datasource full path for SQL Server template datasource provider.");
        }
    }

    public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
    {
        var tableArtifact = this.Artifact as TableArtifact;
        var results = new List<dynamic>();

        if (tableArtifact?.Parent is not RelationalDatabaseDatasourceArtifact datasource)
        {
            Logger.LogWarning("TableArtifactData is only supported for SQL Server datasources");
            return results;
        }

        var sqlServerDatabase = datasource.GetDomainRelationalDatabase();
        if (sqlServerDatabase == null)
            throw new InvalidOperationException("Failed to get SQL Server database from datasource.");

        var columnNames = tableArtifact.GetColumns().Select(c => c.Name).ToList();
        // Build SELECT query
        var query = sqlServerDatabase.GenerateSelectStatement(
            tableName: tableArtifact.Name,
            columnNames: columnNames,
            schema: tableArtifact.Schema,
            whereClause: filter,
            limit: maxRows);

        Logger.LogDebug("Executing query: {Query}", query);

        try
        {
            await using var connection = new SqlConnection(datasource.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(query, connection);
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
