using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Domain.Databases.RelationalDatabases
{
    /// <summary>
    /// SQL Server database definition
    /// </summary>
    public class SqlServerDatabase : RelationalDatabase
    {
        private static readonly Lazy<SqlServerDatabase> _instance = new(() => new SqlServerDatabase());
        public static SqlServerDatabase Instance => _instance.Value;

        public override string Id => "sqlserver";
        public override string Name => "SQL Server";
        public override string Vendor => "Microsoft";
        public override string IdentifierFormat => "[{0}]";
        private readonly List<DatabaseVersion> _versions;
        public override IReadOnlyList<DatabaseVersion> Versions => _versions;

        public override DatabaseVersion DefaultVersion => _versions.First(v => v.Version == "2022");

        //private readonly List<DataTypeMapping> _dataTypeMappings;
        //public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        public override string? DefaultCharacterSet => "UTF-8";
        public override string? DefaultCollation => "SQL_Latin1_General_CP1_CI_AS";
        public override int MaxColumnNameLength => 128;
        public override int MaxTableNameLength => 128;
        public override bool SupportsGeneratedColumns => true;
        public override bool SupportsWindowFunctions => true;
        public override bool SupportsJsonDataType => true;
        public override bool SupportsXmlDataType => true;

        private SqlServerDatabase()
        {
            _versions = new List<DatabaseVersion>
            {
                new("2022", "SQL Server 2022") { ReleaseDate = new DateTime(2022, 11, 16), IsSupported = true },
                new("2019", "SQL Server 2019") { ReleaseDate = new DateTime(2019, 11, 4), IsSupported = true },
                new("2017", "SQL Server 2017") { ReleaseDate = new DateTime(2017, 10, 2), IsSupported = true },
                new("2016", "SQL Server 2016") { ReleaseDate = new DateTime(2016, 6, 1), IsSupported = false },
            };
        }
        protected override DataTypeMappings CreateDataTypeMappings()
        {
            return new SqlServerDataTypeMappings();
        }

        /// <summary>
        /// Generate a CREATE TABLE statement for SQL Server
        /// </summary>
        public override string GenerateCreateTableStatement(
            string tableName,
            string schema,
            IEnumerable<(string name, string type, bool isNullable, bool isPrimaryKey)> columns,
            string? charset = null,
            string? collation = null)
        {
            var sb = new System.Text.StringBuilder();

            var schemaPrefix = !string.IsNullOrEmpty(schema) ? $"{schema}." : "";
            sb.AppendLine($"CREATE TABLE {schemaPrefix}[{tableName}] (");

            var columnLines = new List<string>();
            var primaryKeys = new List<string>();

            foreach (var (name, type, isNullable, isPrimaryKey) in columns)
            {
                var columnDef = $"[{name}]";
                columnDef += " " + type;

                if (!isNullable)
                {
                    columnDef += " NOT NULL";
                }
                else
                {
                    columnDef += " NULL";
                }

                if (isPrimaryKey)
                {
                    primaryKeys.Add(name);
                }

                columnLines.Add("  " + columnDef);
            }

            // Add primary key constraint
            if (primaryKeys.Any())
            {
                columnLines.Add($"  CONSTRAINT PK_{tableName} PRIMARY KEY ({string.Join(", ", primaryKeys.Select(pk => $"[{pk}]"))})");
            }

            sb.AppendLine(string.Join(",\n", columnLines));
            sb.AppendLine(");");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a CREATE INDEX statement for SQL Server
        /// </summary>
        public override string GenerateCreateIndexStatement(
            string tableName,
            string indexName,
            IEnumerable<string> columnNames,
            bool isUnique = false,
            string? schema = null)
        {
            var schemaPrefix = !string.IsNullOrEmpty(schema) ? $"{schema}." : "";
            var uniqueKeyword = isUnique ? "UNIQUE " : "";
            var columns = string.Join(", ", columnNames.Select(c => $"[{c}]"));

            return $"CREATE {uniqueKeyword}INDEX [{indexName}] ON {schemaPrefix}[{tableName}] ({columns});";
        }

        /// <summary>
        /// Basic SELECT statement generator for SQL Server
        /// </summary>
        public override string GenerateSelectStatement(string tableName, IEnumerable<string>? columnNames = null, string? schema = null, string? whereClause = null, int? limit = null)
        {
            var sb = new System.Text.StringBuilder();
            
            var schemaPrefix = !string.IsNullOrEmpty(schema)
                ? $"{EscapeIdentifier(schema)}."
                : "";
            
            var fullTableName = $"{schemaPrefix}{EscapeIdentifier(tableName)}";
            
            var columnsPart = columnNames != null && columnNames.Any()
                ? string.Join(", ", columnNames.Select(c => EscapeIdentifier(c)))
                : "*";
            
            // SQL Server uses TOP instead of LIMIT
            var topClause = limit.HasValue && limit.Value > 0
                ? $"TOP {limit.Value} "
                : "";
            
            sb.Append($"SELECT {topClause}{columnsPart} FROM {fullTableName}");
            
            if (!string.IsNullOrEmpty(whereClause))
            {
                sb.Append($" WHERE {whereClause}");
            }
            
            return sb.ToString();
        }
    }
}
