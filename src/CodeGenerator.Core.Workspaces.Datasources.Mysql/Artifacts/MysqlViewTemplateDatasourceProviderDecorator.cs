using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Templates;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.Artifacts
{
    public class MysqlViewTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
    {
        public MysqlViewTemplateDatasourceProviderDecorator(ArtifactDecoratorState state) 
            : base(state)
        {

        }

        public MysqlViewTemplateDatasourceProviderDecorator(string key) :
            base(key)
        {
            
        }
        public override void Attach(Artifact artifact)
        {
            if(artifact is not ViewArtifact)
            {
                throw new InvalidOperationException("MysqlViewTemplateDatasourceProviderDecorator can only be attached to ViewArtifact.");
            }
            base.Attach(artifact);
        }
        public override string DisplayName { 
            get 
            {
                var viewArtifact = this.Artifact as ViewArtifact;
                var schemaPrefix = string.IsNullOrEmpty(viewArtifact?.Schema) ? "" : $"{viewArtifact.Schema}.";

                if (viewArtifact?.Parent is RelationalDatabaseDatasourceArtifact mysqlDatasource)
                {
                    return $"{mysqlDatasource.Name} > {schemaPrefix}{viewArtifact.Name}";
                }
                throw new InvalidOperationException("Failed to determine datasource name for MySQL template datasource provider.");
            }
        }

        public override string FullPath { 
        get 
            {
                var viewArtifact = this.Artifact as ViewArtifact;
                var schemaPrefix = string.IsNullOrEmpty(viewArtifact?.Schema) ? "" : $"{viewArtifact.Schema}.";
                if (viewArtifact?.Parent is RelationalDatabaseDatasourceArtifact mysqlDatasource)
                {
                    return $"{mysqlDatasource.Name}.{schemaPrefix}{viewArtifact.Name}";
                }
                throw new InvalidOperationException("Failed to determine datasource full path for MySQL template datasource provider.");
            }
        }

        public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
        {
            var viewArtifact = this.Artifact as ViewArtifact;
            var results = new List<dynamic>();

            if (viewArtifact?.Parent is not RelationalDatabaseDatasourceArtifact mysqlDatasource)
            {
                Logger.LogWarning("ViewArtifactData is only supported for MySQL datasources currently");
                return results;
            }

            var mysqlDatabase = mysqlDatasource.GetDomainRelationalDatabase();
            if (mysqlDatabase == null)
                throw new InvalidOperationException("Failed to get MySQL database from datasource.");

            var columnNames = viewArtifact.GetColumns().Select(c => c.Name).ToList();
            // Build SELECT query
            var query = mysqlDatabase.GenerateSelectStatement(tableName: viewArtifact.Name, columnNames: columnNames, viewArtifact.Schema, filter, maxRows);

            Logger.LogDebug("Executing query: {Query}", query);

            try
            {
                await using var connection = new MySqlConnection(mysqlDatasource.ConnectionString);
                await connection.OpenAsync(cancellationToken);

                await using var command = new MySqlCommand(query, connection);
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
}
