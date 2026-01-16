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
    public class MysqlTableTemplateDatasourceProviderDecorator : TemplateDatasourceProviderDecorator
    {
        public MysqlTableTemplateDatasourceProviderDecorator(ArtifactDecoratorState state) 
            : base(state)
        {

        }

        public MysqlTableTemplateDatasourceProviderDecorator(string key) :
            base(key)
        {
            
        }
        public override void Attach(Artifact artifact)
        {
            if(artifact is not TableArtifact)
            {
                throw new InvalidOperationException("MysqlTableTemplateDatasourceProviderDecorator can only be attached to TableArtifact.");
            }
            base.Attach(artifact);
        }
        public override string DisplayName { 
            get 
            {
                var tableArtifact = this.Artifact as TableArtifact;
                var schemaPrefix = string.IsNullOrEmpty(tableArtifact?.Schema) ? "" : $"{tableArtifact.Schema}.";

                if (tableArtifact?.Parent is RelationalDatabaseDatasourceArtifact mysqlDatasource)
                {
                    return $"{mysqlDatasource.Name} > {schemaPrefix}{tableArtifact.Name}";
                }
                throw new InvalidOperationException("Failed to determine datasource name for MySQL template datasource provider.");
            }
        }

        public override string FullPath { 
        get 
            {
                var tableArtifact = this.Artifact as TableArtifact;
                var schemaPrefix = string.IsNullOrEmpty(tableArtifact?.Schema) ? "" : $"{tableArtifact.Schema}.";
                if (tableArtifact?.Parent is RelationalDatabaseDatasourceArtifact mysqlDatasource)
                {
                    return $"{mysqlDatasource.Name}.{schemaPrefix}{tableArtifact.Name}";
                }
                throw new InvalidOperationException("Failed to determine datasource full path for MySQL template datasource provider.");
            }
        }

        public override async Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken)
        {
            var tableArtifact = this.Artifact as TableArtifact;
            var results = new List<dynamic>();

            if (tableArtifact?.Parent is not RelationalDatabaseDatasourceArtifact mysqlDatasource)
            {
                Logger.LogWarning("TableArtifactData is only supported for MySQL datasources currently");
                return results;
            }

            var mysqlDatabase = mysqlDatasource.GetDomainRelationalDatabase();
            if (mysqlDatabase == null)
                throw new InvalidOperationException("Failed to get MySQL database from datasource.");

            var columnNames = tableArtifact.GetColumns().Select(c => c.Name).ToList();
            // Build SELECT query
            var query = mysqlDatabase.GenerateSelectStatement(tableName: tableArtifact.Name, columnNames: columnNames, tableArtifact.Schema, filter, maxRows);

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
}
