using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.Decorators;
using CodeGenerator.Domain.Databases.RelationalDatabases;
using MySqlConnector;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.Services
{
    /// <summary>
    /// Service for retrieving schema information from a MySQL/MariaDB database
    /// </summary>
    public class MysqlSchemaReader
    {
        /// <summary>
        /// Get all tables and views from the database
        /// </summary>
        public async Task<List<DatabaseObjectInfo>> GetTablesAndViewsAsync(
            string connectionString, 
            CancellationToken cancellationToken = default)
        {
            var result = new List<DatabaseObjectInfo>();

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var database = connection.Database;

            // Get tables
            var tablesQuery = @"
                SELECT TABLE_NAME, TABLE_SCHEMA, 'TABLE' as OBJECT_TYPE
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = @database AND TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_NAME";

            await using (var cmd = new MySqlCommand(tablesQuery, connection))
            {
                cmd.Parameters.AddWithValue("@database", database);
                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    result.Add(new DatabaseObjectInfo
                    {
                        Name = reader.GetString("TABLE_NAME"),
                        Schema = reader.GetString("TABLE_SCHEMA"),
                        ObjectType = DatabaseObjectType.Table
                    });
                }
            }

            // Get views
            var viewsQuery = @"
                SELECT TABLE_NAME, TABLE_SCHEMA, 'VIEW' as OBJECT_TYPE
                FROM INFORMATION_SCHEMA.VIEWS 
                WHERE TABLE_SCHEMA = @database
                ORDER BY TABLE_NAME";

            await using (var cmd = new MySqlCommand(viewsQuery, connection))
            {
                cmd.Parameters.AddWithValue("@database", database);
                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    result.Add(new DatabaseObjectInfo
                    {
                        Name = reader.GetString("TABLE_NAME"),
                        Schema = reader.GetString("TABLE_SCHEMA"),
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
            var decorator = table.AddDecorator(new ExistingMysqlTableDecorator
            {
                OriginalTableName = tableName,
                OriginalSchema = schema,
                ImportedAt = DateTime.UtcNow,
                SourceDatasourceName = datasourceName
            });

            await using var connection = new MySqlConnection(connectionString);
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
            view.AddDecorator(new ExistingMysqlViewDecorator
            {
                OriginalViewName = viewName,
                OriginalSchema = schema,
                ImportedAt = DateTime.UtcNow,
                SourceDatasourceName = datasourceName
            });

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            // Get columns (views have the same column structure as tables in INFORMATION_SCHEMA)
            await ImportColumnsAsync(connection, view, viewName, schema, cancellationToken);

            return view;
        }

        private async Task ImportColumnsAsync(
            MySqlConnection connection,
            TableArtifact table,
            string tableName,
            string schema,
            CancellationToken cancellationToken)
        {
            var query = @"
                SELECT 
                    COLUMN_NAME,
                    DATA_TYPE,
                    COLUMN_TYPE,
                    IS_NULLABLE,
                    COLUMN_KEY,
                    EXTRA,
                    CHARACTER_MAXIMUM_LENGTH,
                    NUMERIC_PRECISION,
                    NUMERIC_SCALE,
                    COLUMN_DEFAULT,
                    ORDINAL_POSITION
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tableName
                ORDER BY ORDINAL_POSITION";

            await using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@schema", schema);
            cmd.Parameters.AddWithValue("@tableName", tableName);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var columnName = reader.GetString("COLUMN_NAME");
                // Use COLUMN_TYPE for full type info
                // eg. varchar(255), int(11) unsigned, enum('a','b','c')
                var columnType = reader.GetString("COLUMN_TYPE");
                var dataType = reader.GetString("DATA_TYPE");
                // try to map datatype to generic type
                var typeMapping = MysqlDatabase.Instance.DataTypeMappings.AllMappings.FirstOrDefault(m => string.Equals(m.NativeTypeName, dataType, StringComparison.OrdinalIgnoreCase));
                if(typeMapping != null)
                {
                    columnType = typeMapping.GenericType.Id;
                }
                var isNullable = reader.GetString("IS_NULLABLE") == "YES";
                var columnKey = reader.GetString("COLUMN_KEY");
                var extra = reader.GetString("EXTRA");

                var column = new ColumnArtifact(columnName, columnType, isNullable)
                {
                    IsPrimaryKey = columnKey == "PRI",
                    IsAutoIncrement = extra.Contains("auto_increment", StringComparison.OrdinalIgnoreCase),
                    MaxLength = reader.IsDBNull(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH")) 
                        ? null 
                        : (int?)reader.GetInt64("CHARACTER_MAXIMUM_LENGTH"),
                    Precision = reader.IsDBNull(reader.GetOrdinal("NUMERIC_PRECISION")) 
                        ? null 
                        : (int?)reader.GetInt32("NUMERIC_PRECISION"),
                    Scale = reader.IsDBNull(reader.GetOrdinal("NUMERIC_SCALE")) 
                        ? null 
                        : (int?)reader.GetInt32("NUMERIC_SCALE"),
                    DefaultValue = reader.IsDBNull(reader.GetOrdinal("COLUMN_DEFAULT")) 
                        ? null 
                        : reader.GetString("COLUMN_DEFAULT")
                };

                // Add decorator to mark as existing
                column.AddDecorator(new ExistingMysqlColumnDecorator
                {
                    OriginalColumnName = columnName,
                    OriginalDataType = dataType,// use DATA_TYPE for base type, eg. varchar, int, enum
                    OriginalOrdinalPosition = reader.GetInt32("ORDINAL_POSITION")
                });

                table.AddChild(column);
            }
        }

        private async Task ImportColumnsAsync(
            MySqlConnection connection,
            ViewArtifact view,
            string viewName,
            string schema,
            CancellationToken cancellationToken)
        {
            var query = @"
                SELECT 
                    COLUMN_NAME,
                    DATA_TYPE,
                    COLUMN_TYPE,
                    IS_NULLABLE,
                    CHARACTER_MAXIMUM_LENGTH,
                    NUMERIC_PRECISION,
                    NUMERIC_SCALE,
                    ORDINAL_POSITION
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @viewName
                ORDER BY ORDINAL_POSITION";

            await using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@schema", schema);
            cmd.Parameters.AddWithValue("@viewName", viewName);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var columnName = reader.GetString("COLUMN_NAME");
                var dataType = reader.GetString("COLUMN_TYPE");
                var isNullable = reader.GetString("IS_NULLABLE") == "YES";

                var column = new ColumnArtifact(columnName, dataType, isNullable)
                {
                    MaxLength = reader.IsDBNull(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH")) 
                        ? null 
                        : (int?)reader.GetInt64("CHARACTER_MAXIMUM_LENGTH"),
                    Precision = reader.IsDBNull(reader.GetOrdinal("NUMERIC_PRECISION")) 
                        ? null 
                        : (int?)reader.GetInt32("NUMERIC_PRECISION"),
                    Scale = reader.IsDBNull(reader.GetOrdinal("NUMERIC_SCALE")) 
                        ? null 
                        : (int?)reader.GetInt32("NUMERIC_SCALE")
                };

                // Add decorator to mark as existing
                column.AddDecorator(new ExistingMysqlColumnDecorator
                {
                    OriginalColumnName = columnName,
                    OriginalDataType = reader.GetString("DATA_TYPE"),
                    OriginalOrdinalPosition = reader.GetInt32("ORDINAL_POSITION")
                });

                view.AddChild(column);
            }
        }

        private async Task ImportIndexesAsync(
            MySqlConnection connection,
            TableArtifact table,
            string tableName,
            string schema,
            CancellationToken cancellationToken)
        {
            var query = @"
                SELECT 
                    INDEX_NAME,
                    NON_UNIQUE,
                    INDEX_TYPE,
                    GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX) as COLUMNS
                FROM INFORMATION_SCHEMA.STATISTICS
                WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tableName
                GROUP BY INDEX_NAME, NON_UNIQUE, INDEX_TYPE
                ORDER BY INDEX_NAME";

            await using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@schema", schema);
            cmd.Parameters.AddWithValue("@tableName", tableName);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var indexName = reader.GetString("INDEX_NAME");
                
                // Skip PRIMARY key index as it's represented in columns
                if (indexName == "PRIMARY")
                    continue;

                var isUnique = reader.GetInt32("NON_UNIQUE") == 0;
                var indexType = reader.GetString("INDEX_TYPE");
                var columns = reader.GetString("COLUMNS");

                var index = new IndexArtifact(indexName, isUnique);
                
                // Add columns to index
                foreach (var columnName in columns.Split(','))
                {
                    index.AddColumn(columnName.Trim());
                }

                // Add decorator to mark as existing
                index.AddDecorator(new ExistingMysqlIndexDecorator
                {
                    OriginalIndexName = indexName,
                    OriginalIndexType = indexType
                });

                table.AddChild(index);
            }
        }
    }

    /// <summary>
    /// Information about a database object (table or view)
    /// </summary>
    public class DatabaseObjectInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Schema { get; set; } = string.Empty;
        public DatabaseObjectType ObjectType { get; set; }

        public string DisplayName => $"{Schema}.{Name}";
    }

    public enum DatabaseObjectType
    {
        Table,
        View
    }
}
