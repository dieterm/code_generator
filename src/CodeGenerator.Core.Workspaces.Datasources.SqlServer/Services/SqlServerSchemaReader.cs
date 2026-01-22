using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.Databases.RelationalDatabases;
using Microsoft.Data.SqlClient;

namespace CodeGenerator.Core.Workspaces.Datasources.SqlServer.Services;

/// <summary>
/// Service for retrieving schema information from a SQL Server database
/// </summary>
public class SqlServerSchemaReader
{
    /// <summary>
    /// Get all tables and views from the database
    /// </summary>
    public async Task<List<DatabaseObjectInfo>> GetTablesAndViewsAsync(
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        var result = new List<DatabaseObjectInfo>();

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        // Get tables
        var tablesQuery = @"
            SELECT TABLE_NAME, TABLE_SCHEMA
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_TYPE = 'BASE TABLE'
            ORDER BY TABLE_SCHEMA, TABLE_NAME";

        await using (var cmd = new SqlCommand(tablesQuery, connection))
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
            SELECT TABLE_NAME, TABLE_SCHEMA
            FROM INFORMATION_SCHEMA.VIEWS
            ORDER BY TABLE_SCHEMA, TABLE_NAME";

        await using (var cmd = new SqlCommand(viewsQuery, connection))
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
    /// Import a table with its columns, indexes, and foreign keys
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

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        // Get columns
        await ImportColumnsAsync(connection, table, tableName, schema, cancellationToken);

        // Get indexes
        await ImportIndexesAsync(connection, table, tableName, schema, cancellationToken);

        // Get foreign keys
        await ImportForeignKeysAsync(connection, table, tableName, schema, datasourceName, cancellationToken);

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

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        // Get columns
        await ImportColumnsAsync(connection, view, viewName, schema, cancellationToken);

        return view;
    }

    private async Task ImportColumnsAsync(
        SqlConnection connection,
        TableArtifact table,
        string tableName,
        string schema,
        CancellationToken cancellationToken)
    {
        var query = @"
            SELECT 
                c.COLUMN_NAME,
                c.DATA_TYPE,
                c.IS_NULLABLE,
                c.CHARACTER_MAXIMUM_LENGTH,
                c.NUMERIC_PRECISION,
                c.NUMERIC_SCALE,
                c.COLUMN_DEFAULT,
                c.ORDINAL_POSITION,
                CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IS_PRIMARY_KEY,
                COLUMNPROPERTY(OBJECT_ID(@schema + '.' + @tableName), c.COLUMN_NAME, 'IsIdentity') AS IS_IDENTITY
            FROM INFORMATION_SCHEMA.COLUMNS c
            LEFT JOIN (
                SELECT ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                    ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                    AND tc.TABLE_SCHEMA = ku.TABLE_SCHEMA
                WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                    AND tc.TABLE_SCHEMA = @schema
                    AND tc.TABLE_NAME = @tableName
            ) pk ON c.COLUMN_NAME = pk.COLUMN_NAME
            WHERE c.TABLE_SCHEMA = @schema AND c.TABLE_NAME = @tableName
            ORDER BY c.ORDINAL_POSITION";

        await using var cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@tableName", tableName);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var columnName = reader.GetString(0);
            var dataType = reader.GetString(1);
            
            // Try to map datatype to generic type
            var typeMapping = SqlServerDatabase.Instance.DataTypeMappings.AllMappings
                .FirstOrDefault(m => string.Equals(m.NativeTypeName, dataType, StringComparison.OrdinalIgnoreCase));
            var columnType = typeMapping?.GenericType.Id ?? dataType;
            
            var isNullable = reader.GetString(2) == "YES";
            var isPrimaryKey = reader.GetInt32(8) == 1;
            var isIdentity = reader.IsDBNull(9) ? false : reader.GetInt32(9) == 1;

            var column = new ColumnArtifact(columnName, columnType, isNullable)
            {
                IsPrimaryKey = isPrimaryKey,
                IsAutoIncrement = isIdentity,
                MaxLength = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                Precision = reader.IsDBNull(4) ? null : (int?)reader.GetByte(4),
                Scale = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                DefaultValue = reader.IsDBNull(6) ? null : reader.GetString(6),
                OrdinalPosition = reader.GetInt32(7)
            };

            // Add decorator to mark as existing
            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = columnName,
                OriginalDataType = dataType,
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
        SqlConnection connection,
        ViewArtifact view,
        string viewName,
        string schema,
        CancellationToken cancellationToken)
    {
        var query = @"
            SELECT 
                COLUMN_NAME,
                DATA_TYPE,
                IS_NULLABLE,
                CHARACTER_MAXIMUM_LENGTH,
                NUMERIC_PRECISION,
                NUMERIC_SCALE,
                ORDINAL_POSITION
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @viewName
            ORDER BY ORDINAL_POSITION";

        await using var cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@viewName", viewName);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var columnName = reader.GetString(0);
            var dataType = reader.GetString(1);
            
            // Try to map datatype to generic type
            var typeMapping = SqlServerDatabase.Instance.DataTypeMappings.AllMappings
                .FirstOrDefault(m => string.Equals(m.NativeTypeName, dataType, StringComparison.OrdinalIgnoreCase));
            var columnType = typeMapping?.GenericType.Id ?? dataType;
            
            var isNullable = reader.GetString(2) == "YES";

            var column = new ColumnArtifact(columnName, columnType, isNullable)
            {
                MaxLength = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                Precision = reader.IsDBNull(4) ? null : (int?)reader.GetByte(4),
                Scale = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                OrdinalPosition = reader.GetInt32(6)
            };

            // Add decorator to mark as existing
            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = columnName,
                OriginalDataType = dataType,
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
        SqlConnection connection,
        TableArtifact table,
        string tableName,
        string schema,
        CancellationToken cancellationToken)
    {
        var query = @"
            SELECT 
                i.name AS INDEX_NAME,
                i.is_unique AS IS_UNIQUE,
                i.type_desc AS INDEX_TYPE,
                STRING_AGG(c.name, ',') WITHIN GROUP (ORDER BY ic.key_ordinal) AS COLUMNS
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            INNER JOIN sys.tables t ON i.object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE s.name = @schema 
                AND t.name = @tableName
                AND i.is_primary_key = 0
                AND i.type > 0
            GROUP BY i.name, i.is_unique, i.type_desc
            ORDER BY i.name";

        await using var cmd = new SqlCommand(query, connection);
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

    private async Task ImportForeignKeysAsync(
        SqlConnection connection,
        TableArtifact table,
        string tableName,
        string schema,
        string datasourceName,
        CancellationToken cancellationToken)
    {
        var query = @"
            SELECT 
                fk.name AS constraint_name,
                COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS column_name,
                SCHEMA_NAME(ref_t.schema_id) AS referenced_schema,
                ref_t.name AS referenced_table,
                COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS referenced_column,
                fk.update_referential_action_desc AS update_action,
                fk.delete_referential_action_desc AS delete_action,
                fkc.constraint_column_id AS ordinal_position
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            INNER JOIN sys.tables ref_t ON fk.referenced_object_id = ref_t.object_id
            WHERE s.name = @schema AND t.name = @tableName
            ORDER BY fk.name, fkc.constraint_column_id";

        var foreignKeyData = new Dictionary<string, ForeignKeyInfo>();

        await using var cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@tableName", tableName);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var constraintName = reader.GetString(0);
            var columnName = reader.GetString(1);
            var referencedSchema = reader.GetString(2);
            var referencedTable = reader.GetString(3);
            var referencedColumn = reader.GetString(4);
            var updateAction = reader.GetString(5);
            var deleteAction = reader.GetString(6);

            if (!foreignKeyData.TryGetValue(constraintName, out var fkInfo))
            {
                fkInfo = new ForeignKeyInfo
                {
                    ConstraintName = constraintName,
                    ReferencedTableSchema = referencedSchema,
                    ReferencedTableName = referencedTable,
                    OnUpdateAction = ParseSqlServerForeignKeyAction(updateAction),
                    OnDeleteAction = ParseSqlServerForeignKeyAction(deleteAction),
                    ColumnMappings = new List<(string SourceColumn, string ReferencedColumn)>()
                };
                foreignKeyData[constraintName] = fkInfo;
            }

            fkInfo.ColumnMappings.Add((columnName, referencedColumn));
        }

        // Create ForeignKeyArtifact for each foreign key
        foreach (var fkInfo in foreignKeyData.Values)
        {
            var foreignKey = new ForeignKeyArtifact(fkInfo.ConstraintName)
            {
                OnDeleteAction = fkInfo.OnDeleteAction,
                OnUpdateAction = fkInfo.OnUpdateAction
            };

            var columnMappings = new List<ForeignKeyColumnMapping>();
            var originalColumnMappings = new List<ExistingForeignKeyColumnMapping>();

            foreach (var (sourceColumnName, referencedColumnName) in fkInfo.ColumnMappings)
            {
                var sourceColumn = table.GetColumns().FirstOrDefault(c => c.Name == sourceColumnName);
                if (sourceColumn != null)
                {
                    columnMappings.Add(new ForeignKeyColumnMapping(sourceColumn.Id, string.Empty));
                }

                originalColumnMappings.Add(new ExistingForeignKeyColumnMapping(sourceColumnName, referencedColumnName));
            }

            foreignKey.ColumnMappings = columnMappings;

            foreignKey.AddDecorator(new ExistingForeignKeyDecorator
            {
                OriginalName = fkInfo.ConstraintName,
                OriginalSourceTableName = tableName,
                OriginalSourceTableSchema = schema,
                OriginalReferencedTableName = fkInfo.ReferencedTableName,
                OriginalReferencedTableSchema = fkInfo.ReferencedTableSchema,
                OriginalOnDeleteAction = fkInfo.OnDeleteAction,
                OriginalOnUpdateAction = fkInfo.OnUpdateAction,
                OriginalColumnMappings = originalColumnMappings,
                ImportedAt = DateTime.UtcNow,
                SourceDatasourceName = datasourceName
            });

            table.AddChild(foreignKey);
        }
    }

    private static ForeignKeyAction ParseSqlServerForeignKeyAction(string action)
    {
        return action.ToUpperInvariant() switch
        {
            "CASCADE" => ForeignKeyAction.Cascade,
            "SET_NULL" => ForeignKeyAction.SetNull,
            "NO_ACTION" => ForeignKeyAction.NoAction,
            "SET_DEFAULT" => ForeignKeyAction.NoAction, // Map SET_DEFAULT to NoAction as we don't have a SetDefault enum value
            _ => ForeignKeyAction.NoAction
        };
    }

    private class ForeignKeyInfo
    {
        public string ConstraintName { get; set; } = string.Empty;
        public string ReferencedTableSchema { get; set; } = string.Empty;
        public string ReferencedTableName { get; set; } = string.Empty;
        public ForeignKeyAction OnDeleteAction { get; set; }
        public ForeignKeyAction OnUpdateAction { get; set; }
        public List<(string SourceColumn, string ReferencedColumn)> ColumnMappings { get; set; } = new();
    }
}
