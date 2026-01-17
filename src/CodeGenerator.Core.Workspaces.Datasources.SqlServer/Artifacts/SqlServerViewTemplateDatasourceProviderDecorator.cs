using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace CodeGenerator.Core.Workspaces.Datasources.SqlServer.Artifacts;

public class SqlServerViewTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
{
    public SqlServerViewTemplateDatasourceProviderDecorator(ArtifactDecoratorState state)
        : base(state)
    {
    }

    public SqlServerViewTemplateDatasourceProviderDecorator(string key)
        : base(key)
    {
    }

    public override void Attach(Artifact artifact)
    {
        if (artifact is not ViewArtifact)
        {
            throw new InvalidOperationException("SqlServerViewTemplateDatasourceProviderDecorator can only be attached to ViewArtifact.");
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
            throw new InvalidOperationException("Failed to determine datasource name for SQL Server template datasource provider.");
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
            throw new InvalidOperationException("Failed to determine datasource full path for SQL Server template datasource provider.");
        }
    }

    public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
    {
        var viewArtifact = this.Artifact as ViewArtifact;
        var results = new List<dynamic>();

        if (viewArtifact?.Parent is not RelationalDatabaseDatasourceArtifact datasource)
        {
            Logger.LogWarning("ViewArtifactData is only supported for SQL Server datasources");
            return results;
        }

        var sqlServerDatabase = datasource.GetDomainRelationalDatabase();
        if (sqlServerDatabase == null)
            throw new InvalidOperationException("Failed to get SQL Server database from datasource.");

        var columnNames = viewArtifact.GetColumns().Select(c => c.Name).ToList();
        // Build SELECT query
        var query = sqlServerDatabase.GenerateSelectStatement(
            tableName: viewArtifact.Name,
            columnNames: columnNames,
            schema: viewArtifact.Schema,
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
