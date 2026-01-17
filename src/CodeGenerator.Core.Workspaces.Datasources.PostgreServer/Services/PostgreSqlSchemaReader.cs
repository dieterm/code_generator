using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.Databases.RelationalDatabases;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Services;

/// <summary>
/// Service for retrieving schema information from a PostgreSQL database
/// </summary>
public class PostgreSqlSchemaReader
{
    private readonly ILogger<PostgreSqlSchemaReader> _logger;
    public PostgreSqlSchemaReader(ILogger<PostgreSqlSchemaReader> logger)
    {
        _logger = logger;
    }
    /// <summary>
    /// Get all tables and views from the database
    /// </summary>
    public async Task<List<DatabaseObjectInfo>> GetTablesAndViewsAsync(
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        var result = new List<DatabaseObjectInfo>();

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        // Get tables
        var tablesQuery = @"
            SELECT table_name, table_schema
            FROM information_schema.tables 
            WHERE table_type = 'BASE TABLE'
              AND table_schema NOT IN ('pg_catalog', 'information_schema')
            ORDER BY table_schema, table_name";

        await using (var cmd = new NpgsqlCommand(tablesQuery, connection))
        {
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new DatabaseObjectInfo
                {
                    Name = reader.GetString(0),
                    Schema = reader.GetString(1),
                    ObjectType = DatabaseObjectType.Table
                });
            }
        }

        // Get views
        var viewsQuery = @"
            SELECT table_name, table_schema
            FROM information_schema.views
            WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
            ORDER BY table_schema, table_name";

        await using (var cmd = new NpgsqlCommand(viewsQuery, connection))
        {
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new DatabaseObjectInfo
                {
                    Name = reader.GetString(0),
                    Schema = reader.GetString(1),
                    ObjectType = DatabaseObjectType.View
                });
            }
        }

        return result;
    }

    /// <summary>
    /// Import a table with its columns and indexes
    /// </summary>
    public async Task<TableArtifact> ImportTableAsync(
        string connectionString,
        string tableName,
        string schema,
        string datasourceName,
        CancellationToken cancellationToken = default)
    {
        var table = new TableArtifact(tableName, schema);

        // Add decorator to mark as existing
        table.AddDecorator(new ExistingTableDecorator
        {
            OriginalTableName = tableName,
            OriginalSchema = schema,
            ImportedAt = DateTime.UtcNow,
            SourceDatasourceName = datasourceName
        });

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        // Get columns
        await ImportColumnsAsync(connection, table, tableName, schema, cancellationToken);

        // Get indexes
        await ImportIndexesAsync(connection, table, tableName, schema, cancellationToken);

        return table;
    }

    /// <summary>
    /// Import a view with its columns
    /// </summary>
    public async Task<ViewArtifact> ImportViewAsync(
        string connectionString,
        string viewName,
        string schema,
        string datasourceName,
        CancellationToken cancellationToken = default)
    {
        var view = new ViewArtifact(viewName, schema);

        // Add decorator to mark as existing
        view.AddDecorator(new ExistingViewDecorator
        {
            OriginalViewName = viewName,
            OriginalSchema = schema,
            ImportedAt = DateTime.UtcNow,
            SourceDatasourceName = datasourceName
        });

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        // Get columns
        await ImportColumnsAsync(connection, view, viewName, schema, cancellationToken);

        return view;
    }

    private async Task ImportColumnsAsync(
        NpgsqlConnection connection,
        TableArtifact table,
        string tableName,
        string schema,
        CancellationToken cancellationToken)
    {
        var query = @"
            SELECT 
                c.column_name,
                c.data_type,
                c.udt_name,
                c.is_nullable,
                c.character_maximum_length,
                c.numeric_precision,
                c.numeric_scale,
                c.column_default,
                c.ordinal_position,
                CASE WHEN pk.column_name IS NOT NULL THEN true ELSE false END AS is_primary_key,
                CASE WHEN c.column_default LIKE 'nextval%' THEN true ELSE false END AS is_serial
            FROM information_schema.columns c
            LEFT JOIN (
                SELECT kcu.column_name
                FROM information_schema.table_constraints tc
                JOIN information_schema.key_column_usage kcu 
                    ON tc.constraint_name = kcu.constraint_name
                    AND tc.table_schema = kcu.table_schema
                WHERE tc.constraint_type = 'PRIMARY KEY'
                    AND tc.table_schema = @schema
                    AND tc.table_name = @tableName
            ) pk ON c.column_name = pk.column_name
            WHERE c.table_schema = @schema AND c.table_name = @tableName
            ORDER BY c.ordinal_position";

        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@tableName", tableName);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var columnName = reader.GetString(0);
            var dataType = reader.GetString(1);
            var udtName = reader.GetString(2);
            
            // Try to map datatype to generic type
            var typeMapping = PostgreSqlDatabase.Instance.DataTypeMappings.AllMappings
                .FirstOrDefault(m => string.Equals(m.NativeTypeName, udtName, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(m.NativeTypeName, dataType, StringComparison.OrdinalIgnoreCase));
            if(typeMapping==null)
            {
                _logger.LogWarning("No type mapping found for native type '{NativeType}' or data type '{DataType}' in table '{TableName}'", udtName, dataType, tableName);
            }
            var columnType = typeMapping?.GenericType.Id ?? dataType;
            
            var isNullable = reader.GetString(3) == "YES";
            var isPrimaryKey = reader.GetBoolean(9);
            var isSerial = reader.GetBoolean(10);

            var column = new ColumnArtifact(columnName, columnType, isNullable)
            {
                IsPrimaryKey = isPrimaryKey,
                IsAutoIncrement = isSerial,
                MaxLength = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                Precision = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                Scale = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                DefaultValue = reader.IsDBNull(7) ? null : reader.GetString(7),
                OrdinalPosition = reader.GetInt32(8)
            };

            // Add decorator to mark as existing
            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = columnName,
                OriginalDataType = udtName,
                OriginalOrdinalPosition = column.OrdinalPosition,
                OriginalIsNullable = isNullable,
                OriginalIsPrimaryKey = column.IsPrimaryKey,
                OriginalIsAutoIncrement = column.IsAutoIncrement,
                OriginalMaxLength = column.MaxLength,
                OriginalPrecision = column.Precision,
                OriginalScale = column.Scale,
                OriginalDefaultValue = column.DefaultValue
            });

            table.AddChild(column);
        }
    }

    private async Task ImportColumnsAsync(
        NpgsqlConnection connection,
        ViewArtifact view,
        string viewName,
        string schema,
        CancellationToken cancellationToken)
    {
        var query = @"
            SELECT 
                column_name,
                data_type,
                udt_name,
                is_nullable,
                character_maximum_length,
                numeric_precision,
                numeric_scale,
                ordinal_position
            FROM information_schema.columns
            WHERE table_schema = @schema AND table_name = @viewName
            ORDER BY ordinal_position";

        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@viewName", viewName);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var columnName = reader.GetString(0);
            var dataType = reader.GetString(1);
            var udtName = reader.GetString(2);
            
            // Try to map datatype to generic type
            var typeMapping = PostgreSqlDatabase.Instance.DataTypeMappings.AllMappings
                .FirstOrDefault(m => string.Equals(m.NativeTypeName, udtName, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(m.NativeTypeName, dataType, StringComparison.OrdinalIgnoreCase));
            var columnType = typeMapping?.GenericType.Id ?? dataType;
            
            var isNullable = reader.GetString(3) == "YES";

            var column = new ColumnArtifact(columnName, columnType, isNullable)
            {
                MaxLength = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                Precision = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                Scale = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                OrdinalPosition = reader.GetInt32(7)
            };

            // Add decorator to mark as existing
            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = columnName,
                OriginalDataType = udtName,
                OriginalOrdinalPosition = column.OrdinalPosition,
                OriginalIsNullable = isNullable,
                OriginalMaxLength = column.MaxLength,
                OriginalPrecision = column.Precision,
                OriginalScale = column.Scale
            });

            view.AddChild(column);
        }
    }

    private async Task ImportIndexesAsync(
        NpgsqlConnection connection,
        TableArtifact table,
        string tableName,
        string schema,
        CancellationToken cancellationToken)
    {
        var query = @"
            SELECT 
                i.relname AS index_name,
                ix.indisunique AS is_unique,
                am.amname AS index_type,
                string_agg(a.attname, ',' ORDER BY array_position(ix.indkey, a.attnum)) AS columns
            FROM pg_index ix
            JOIN pg_class i ON i.oid = ix.indexrelid
            JOIN pg_class t ON t.oid = ix.indrelid
            JOIN pg_namespace n ON n.oid = t.relnamespace
            JOIN pg_am am ON am.oid = i.relam
            JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = ANY(ix.indkey)
            WHERE n.nspname = @schema 
                AND t.relname = @tableName
                AND NOT ix.indisprimary
            GROUP BY i.relname, ix.indisunique, am.amname
            ORDER BY i.relname";

        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@tableName", tableName);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var indexName = reader.GetString(0);
            var isUnique = reader.GetBoolean(1);
            var indexType = reader.GetString(2);
            var columns = reader.GetString(3);

            var index = new IndexArtifact(indexName, isUnique);

            // Add columns to index
            foreach (var columnName in columns.Split(','))
            {
                index.AddColumn(columnName.Trim());
            }

            // Add decorator to mark as existing
            index.AddDecorator(new ExistingIndexDecorator
            {
                OriginalIndexName = indexName,
                OriginalIndexType = indexType
            });

            table.AddChild(index);
        }
    }
}
