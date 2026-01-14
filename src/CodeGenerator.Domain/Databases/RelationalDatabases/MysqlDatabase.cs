using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Domain.Databases.RelationalDatabases
{
    /// <summary>
    /// MySQL/MariaDB database definition
    /// </summary>
    public class MysqlDatabase : RelationalDatabase
    {
        private static readonly Lazy<MysqlDatabase> _instance = new(() => new MysqlDatabase());
        public static MysqlDatabase Instance => _instance.Value;

        public override string Id => "mysql";
        public override string Name => "MySQL / MariaDB";
        public override string Vendor => "MySQL / MariaDB Foundation";
        public override string IdentifierFormat => "`{0}`";
        private readonly List<DatabaseVersion> _versions;
        public override IReadOnlyList<DatabaseVersion> Versions => _versions;

        public override DatabaseVersion DefaultVersion => _versions.First(v => v.Version == "8.0");

        //private readonly List<DataTypeMapping> _dataTypeMappings;
        //public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private MysqlDatabase()
        {
            _versions = new List<DatabaseVersion>
            {
                new("8.0", "MySQL 8.0") { ReleaseDate = new DateTime(2018, 4, 19), IsSupported = true },
                new("5.7", "MySQL 5.7") { ReleaseDate = new DateTime(2013, 2, 3), IsSupported = true },
                new("5.6", "MySQL 5.6") { ReleaseDate = new DateTime(2011, 10, 3), IsSupported = false },
                new("10.5", "MariaDB 10.5") { ReleaseDate = new DateTime(2020, 1, 22), IsSupported = true },
                new("10.6", "MariaDB 10.6") { ReleaseDate = new DateTime(2021, 6, 24), IsSupported = true },
            };

        }

        protected override DataTypeMappings CreateDataTypeMappings()
        {
            return new MysqlDatatypeMappings();
        }

        /// <summary>
        /// Generate a CREATE TABLE statement for MySQL
        /// </summary>
        public override string GenerateCreateTableStatement(
            string tableName,
            string schema,
            IEnumerable<(string name, string type, bool isNullable, bool isPrimaryKey)> columns,
            string? charset = "utf8mb4",
            string? collation = "utf8mb4_unicode_ci")
        {
            var sb = new System.Text.StringBuilder();

            // CREATE TABLE statement
            var fullTableName = !string.IsNullOrEmpty(schema) && schema != "dbo" 
                ? $"{schema}.{tableName}" 
                : tableName;

            sb.AppendLine($"CREATE TABLE IF NOT EXISTS {fullTableName} (");

            var columnLines = new List<string>();
            var primaryKeys = new List<string>();

            foreach (var (name, type, isNullable, isPrimaryKey) in columns)
            {
                var columnDef = name;
                columnDef += " " + type;

                if (!isNullable)
                {
                    columnDef += " NOT NULL";
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
                columnLines.Add($"  PRIMARY KEY ({string.Join(", ", primaryKeys.Select(pk => $"`{pk}`"))})");
            }

            sb.AppendLine(string.Join(",\n", columnLines));

            // Table options
            sb.Append(") ENGINE=InnoDB");
            
            if (!string.IsNullOrEmpty(charset))
            {
                sb.Append($" DEFAULT CHARSET={charset}");
            }

            if (!string.IsNullOrEmpty(collation))
            {
                sb.Append($" COLLATE {collation}");
            }

            sb.AppendLine(";");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a CREATE INDEX statement for MySQL
        /// </summary>
        public override string GenerateCreateIndexStatement(
            string tableName,
            string indexName,
            IEnumerable<string> columnNames,
            bool isUnique = false,
            string? schema = null)
        {
            var fullTableName = !string.IsNullOrEmpty(schema) && schema != "dbo"
                ? $"{schema}.{tableName}"
                : tableName;

            var uniqueKeyword = isUnique ? "UNIQUE " : "";
            var columns = string.Join(", ", columnNames.Select(c => $"`{c}`"));

            return $"CREATE {uniqueKeyword}INDEX `{indexName}` ON {fullTableName} ({columns});";
        }
    }
}
