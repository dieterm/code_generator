using CodeGenerator.Domain.DataTypes;
using System.Net.NetworkInformation;

namespace CodeGenerator.Domain.Databases.RelationalDatabases
{
    /// <summary>
    /// PostgreSQL database definition
    /// </summary>
    public class PostgreSqlDatabase : RelationalDatabase
    {
        private static readonly Lazy<PostgreSqlDatabase> _instance = new(() => new PostgreSqlDatabase());
        public static PostgreSqlDatabase Instance => _instance.Value;

        public override string Id => "postgresql";
        public override string Name => "PostgreSQL";
        public override string Vendor => "PostgreSQL Global Development Group";

        private readonly List<DatabaseVersion> _versions;
        public override IReadOnlyList<DatabaseVersion> Versions => _versions;

        public override DatabaseVersion DefaultVersion => _versions.First(v => v.Version == "15");

        //private readonly List<DataTypeMapping> _dataTypeMappings;
        //public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        public override string? DefaultCharacterSet => "UTF8";
        public override int MaxColumnNameLength => 63;
        public override int MaxTableNameLength => 63;
        public override bool SupportsGeneratedColumns => true;
        public override bool SupportsWindowFunctions => true;
        public override bool SupportsJsonDataType => true;
        public override bool SupportsXmlDataType => true;

        private PostgreSqlDatabase()
        {
            _versions = new List<DatabaseVersion>
            {
                new("15", "PostgreSQL 15") { ReleaseDate = new DateTime(2022, 10, 13), IsSupported = true },
                new("14", "PostgreSQL 14") { ReleaseDate = new DateTime(2021, 10, 28), IsSupported = true },
                new("13", "PostgreSQL 13") { ReleaseDate = new DateTime(2020, 10, 29), IsSupported = true },
                new("12", "PostgreSQL 12") { ReleaseDate = new DateTime(2019, 10, 3), IsSupported = false },
            };

        }
        protected override DataTypeMappings CreateDataTypeMappings()
        {
            return new PostgreSqlDataTypeMappings();
        }
        /// <summary>
        /// Generate a CREATE TABLE statement for PostgreSQL
        /// </summary>
        public override string GenerateCreateTableStatement(
            string tableName,
            string schema,
            IEnumerable<(string name, string type, bool isNullable, bool isPrimaryKey)> columns,
            string? charset = null,
            string? collation = null)
        {
            var sb = new System.Text.StringBuilder();

            var schemaPrefix = !string.IsNullOrEmpty(schema) && schema != "public" 
                ? $"{schema}." 
                : "";

            sb.AppendLine($"CREATE TABLE {schemaPrefix}\"{tableName}\" (");

            var columnLines = new List<string>();
            var primaryKeys = new List<string>();

            foreach (var (name, type, isNullable, isPrimaryKey) in columns)
            {
                var columnDef = $"\"{name}\"";
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
                columnLines.Add($"  CONSTRAINT pk_{tableName.ToLower()} PRIMARY KEY ({string.Join(", ", primaryKeys.Select(pk => $"\"{pk}\""))})");
            }

            sb.AppendLine(string.Join(",\n", columnLines));
            sb.AppendLine(");");

            // Add table comment
            sb.AppendLine($"COMMENT ON TABLE {schemaPrefix}\"{tableName}\" IS '';");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a CREATE INDEX statement for PostgreSQL
        /// </summary>
        public override string GenerateCreateIndexStatement(
            string tableName,
            string indexName,
            IEnumerable<string> columnNames,
            bool isUnique = false,
            string? schema = null)
        {
            var schemaPrefix = !string.IsNullOrEmpty(schema) && schema != "public"
                ? $"{schema}."
                : "";

            var uniqueKeyword = isUnique ? "UNIQUE " : "";
            var columns = string.Join(", ", columnNames.Select(c => $"\"{c}\""));

            return $"CREATE {uniqueKeyword}INDEX \"{indexName}\" ON {schemaPrefix}\"{tableName}\" ({columns});";
        }
    }
}
