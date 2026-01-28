using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.Databases.RelationalDatabases;
using CodeGenerator.Domain.DataTypes;
using MySqlConnector;
using System.Text.RegularExpressions;

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
            var decorator = table.AddDecorator(new ExistingTableDecorator
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
                
                // Extract allowed values for ENUM type
                string? allowedValues = null;
                if (string.Equals(dataType, "enum", StringComparison.OrdinalIgnoreCase))
                {
                    allowedValues = ExtractEnumValues(columnType);
                }
                
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
                        : reader.GetString("COLUMN_DEFAULT"),
                    OrdinalPosition = reader.GetInt32("ORDINAL_POSITION"),
                    AllowedValues = allowedValues
                };

                // Add decorator to mark as existing
                column.AddDecorator(new ExistingColumnDecorator
                {
                    OriginalName = columnName,
                    OriginalDataType = dataType,// use DATA_TYPE for base type, eg. varchar, int, enum
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
                // Use COLUMN_TYPE for full type info
                // eg. varchar(255), int(11) unsigned, enum('a','b','c')
                var columnType = reader.GetString("COLUMN_TYPE");
                var dataType = reader.GetString("DATA_TYPE");
                
                // Extract allowed values for ENUM type
                string? allowedValues = null;
                if (string.Equals(dataType, "enum", StringComparison.OrdinalIgnoreCase))
                {
                    allowedValues = ExtractEnumValues(columnType);
                }
                
                // try to map datatype to generic type
                var typeMapping = MysqlDatabase.Instance.DataTypeMappings.AllMappings.FirstOrDefault(m => string.Equals(m.NativeTypeName, dataType, StringComparison.OrdinalIgnoreCase));
                if (typeMapping != null)
                {
                    columnType = typeMapping.GenericType.Id;
                }
                var isNullable = reader.GetString("IS_NULLABLE") == "YES";

                var column = new ColumnArtifact(columnName, columnType, isNullable)
                {
                    MaxLength = reader.IsDBNull(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH")) 
                        ? null 
                        : (int?)reader.GetInt64("CHARACTER_MAXIMUM_LENGTH"),
                    Precision = reader.IsDBNull(reader.GetOrdinal("NUMERIC_PRECISION")) 
                        ? null 
                        : (int?)reader.GetInt32("NUMERIC_PRECISION"),
                    Scale = reader.IsDBNull(reader.GetOrdinal("NUMERIC_SCALE")) 
                        ? null 
                        : (int?)reader.GetInt32("NUMERIC_SCALE"),
                    OrdinalPosition = reader.GetInt32("ORDINAL_POSITION"),
                    AllowedValues = allowedValues
                };

                // Add decorator to mark as existing
                column.AddDecorator(new ExistingColumnDecorator
                {
                    OriginalName = columnName,
                    OriginalDataType = dataType,
                    OriginalOrdinalPosition = column.OrdinalPosition,
                    OriginalIsNullable = isNullable,
                    OriginalMaxLength= column.MaxLength,
                    OriginalPrecision = column.Precision,
                    OriginalScale = column.Scale
                });

                view.AddChild(column);
            }
        }

        /// <summary>
        /// Extract enum values from MySQL COLUMN_TYPE like "enum('value1','value2','value3')"
        /// Returns comma-separated values: "value1,value2,value3"
        /// </summary>
        private static string? ExtractEnumValues(string columnType)
        {
            if (string.IsNullOrEmpty(columnType))
                return null;

            // Match enum('value1','value2',...)
            var match = Regex.Match(columnType, @"enum\((.+)\)", RegexOptions.IgnoreCase);
            if (!match.Success)
                return null;

            var valuesString = match.Groups[1].Value;
            
            // Extract individual values (handling escaped quotes)
            var values = new List<string>();
            var valueMatches = Regex.Matches(valuesString, @"'((?:[^'\\]|\\.)*)'");
            foreach (Match valueMatch in valueMatches)
            {
                // Unescape the value
                var value = valueMatch.Groups[1].Value.Replace("\\'", "'").Replace("\\\\", "\\");
                values.Add(value);
            }

            return values.Count > 0 ? string.Join(",", values) : null;
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
                index.AddDecorator(new ExistingIndexDecorator
                {
                    OriginalIndexName = indexName,
                    OriginalIndexType = indexType
                });

                table.AddChild(index);
            }
        }

        private async Task ImportForeignKeysAsync(
            MySqlConnection connection,
            TableArtifact table,
            string tableName,
            string schema,
            string datasourceName,
            CancellationToken cancellationToken)
        {
            // Query to get foreign key information
            var query = @"
                SELECT 
                    kcu.CONSTRAINT_NAME,
                    kcu.COLUMN_NAME,
                    kcu.REFERENCED_TABLE_SCHEMA,
                    kcu.REFERENCED_TABLE_NAME,
                    kcu.REFERENCED_COLUMN_NAME,
                    kcu.ORDINAL_POSITION,
                    rc.UPDATE_RULE,
                    rc.DELETE_RULE
                FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                INNER JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
                    ON kcu.CONSTRAINT_NAME = rc.CONSTRAINT_NAME
                    AND kcu.CONSTRAINT_SCHEMA = rc.CONSTRAINT_SCHEMA
                WHERE kcu.TABLE_SCHEMA = @schema 
                    AND kcu.TABLE_NAME = @tableName
                    AND kcu.REFERENCED_TABLE_NAME IS NOT NULL
                ORDER BY kcu.CONSTRAINT_NAME, kcu.ORDINAL_POSITION";

            var foreignKeyData = new Dictionary<string, ForeignKeyInfo>();

            await using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@schema", schema);
            cmd.Parameters.AddWithValue("@tableName", tableName);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var constraintName = reader.GetString("CONSTRAINT_NAME");
                var columnName = reader.GetString("COLUMN_NAME");
                var referencedTableSchema = reader.GetString("REFERENCED_TABLE_SCHEMA");
                var referencedTableName = reader.GetString("REFERENCED_TABLE_NAME");
                var referencedColumnName = reader.GetString("REFERENCED_COLUMN_NAME");
                var updateRule = reader.GetString("UPDATE_RULE");
                var deleteRule = reader.GetString("DELETE_RULE");

                if (!foreignKeyData.TryGetValue(constraintName, out var fkInfo))
                {
                    fkInfo = new ForeignKeyInfo
                    {
                        ConstraintName = constraintName,
                        ReferencedTableSchema = referencedTableSchema,
                        ReferencedTableName = referencedTableName,
                        OnUpdateAction = ParseForeignKeyAction(updateRule),
                        OnDeleteAction = ParseForeignKeyAction(deleteRule),
                        ColumnMappings = new List<(string SourceColumn, string ReferencedColumn)>()
                    };
                    foreignKeyData[constraintName] = fkInfo;
                }

                fkInfo.ColumnMappings.Add((columnName, referencedColumnName));
            }

            // Create ForeignKeyArtifact for each foreign key
            foreach (var fkInfo in foreignKeyData.Values)
            {
                var foreignKey = new ForeignKeyArtifact(fkInfo.ConstraintName)
                {
                    OnDeleteAction = fkInfo.OnDeleteAction,
                    OnUpdateAction = fkInfo.OnUpdateAction
                };

                // Note: ReferencedTableId will be set later when the referenced table is resolved
                // For now, we store the table name in the decorator

                // Add column mappings (by column ID - we need to find the column artifacts)
                var columnMappings = new List<ForeignKeyColumnMapping>();
                var originalColumnMappings = new List<ExistingForeignKeyColumnMapping>();

                foreach (var (sourceColumnName, referencedColumnName) in fkInfo.ColumnMappings)
                {
                    // Find the source column in the table
                    var sourceColumn = table.GetColumns().FirstOrDefault(c => c.Name == sourceColumnName);
                    if (sourceColumn != null)
                    {
                        // We don't have the referenced column ID yet (referenced table may not be imported)
                        // Store mapping with source column ID, referenced column ID will be empty for now
                        columnMappings.Add(new ForeignKeyColumnMapping(sourceColumn.Id, string.Empty));
                    }

                    // Always store original column names in decorator
                    originalColumnMappings.Add(new ExistingForeignKeyColumnMapping(sourceColumnName, referencedColumnName));
                }

                foreignKey.ColumnMappings = columnMappings;

                // Add decorator to mark as existing
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

        private static ForeignKeyAction ParseForeignKeyAction(string rule)
        {
            return rule.ToUpperInvariant() switch
            {
                "CASCADE" => ForeignKeyAction.Cascade,
                "SET NULL" => ForeignKeyAction.SetNull,
                "RESTRICT" => ForeignKeyAction.Restrict,
                "NO ACTION" => ForeignKeyAction.NoAction,
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
}
