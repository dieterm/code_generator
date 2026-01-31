using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.Rust
{
    /// <summary>
    /// Rust programming language definition
    /// </summary>
    public class RustLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<RustLanguage> _instance = new(() => new RustLanguage());
        public static RustLanguage Instance => _instance.Value;

        public override string Id => "rust";
        public override string Name => "Rust";
        public override string FileExtension => ".rs";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "1.74");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private RustLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("1.74", "Rust 1.74") { ReleaseDate = new DateTime(2023, 11, 16) },
                new("1.73", "Rust 1.73") { ReleaseDate = new DateTime(2023, 10, 5) },
                new("1.72", "Rust 1.72") { ReleaseDate = new DateTime(2023, 8, 24) },
                new("1.71", "Rust 1.71") { ReleaseDate = new DateTime(2023, 7, 13) },
                new("1.70", "Rust 1.70") { ReleaseDate = new DateTime(2023, 6, 1) },
            };

            _namingConventions = new NamingConventions.NamingConventions
            {
                ClassName = NamingStyle.PascalCase,          // struct/enum names
                InterfaceName = NamingStyle.PascalCase,      // trait names
                InterfacePrefix = "",
                MethodName = NamingStyle.SnakeCase,
                PropertyName = NamingStyle.SnakeCase,
                LocalVariableName = NamingStyle.SnakeCase,
                ParameterName = NamingStyle.SnakeCase,
                PrivateFieldName = NamingStyle.SnakeCase,
                PrivateFieldPrefix = "",
                ConstantName = NamingStyle.UpperSnakeCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.PascalCase,
                NamespaceName = NamingStyle.SnakeCase,       // module names
                FileName = NamingStyle.SnakeCase,
                FileExtension = ".rs"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "i8", "i8", "u8"),
                new(GenericDataTypes.SmallInt, "i16", "i16"),
                new(GenericDataTypes.Int, "i32", "i32"),
                new(GenericDataTypes.BigInt, "i64", "i64"),

                // Decimal types
                new(GenericDataTypes.Decimal, "f64", "f64"),
                new(GenericDataTypes.Float, "f32", "f32"),
                new(GenericDataTypes.Double, "f64", "f64"),
                new(GenericDataTypes.Money, "f64", "f64"),

                // String types
                new(GenericDataTypes.Char, "char", "char"),
                new(GenericDataTypes.VarChar, "String", "String"),
                new(GenericDataTypes.NChar, "char", "char"),
                new(GenericDataTypes.NVarChar, "String", "String"),
                new(GenericDataTypes.Text, "String", "String"),
                new(GenericDataTypes.NText, "String", "String"),

                // Boolean types
                new(GenericDataTypes.Boolean, "bool", "bool"),
                new(GenericDataTypes.Bit, "bool", "bool"),

                // Date/Time types (using chrono crate)
                new(GenericDataTypes.Date, "NaiveDate", "chrono::NaiveDate"),
                new(GenericDataTypes.Time, "NaiveTime", "chrono::NaiveTime"),
                new(GenericDataTypes.DateTime, "NaiveDateTime", "chrono::NaiveDateTime"),
                new(GenericDataTypes.DateTime2, "NaiveDateTime", "chrono::NaiveDateTime"),
                new(GenericDataTypes.Timestamp, "DateTime<Utc>", "chrono::DateTime<chrono::Utc>"),
                new(GenericDataTypes.DateTimeOffset, "DateTime<FixedOffset>", "chrono::DateTime<chrono::FixedOffset>"),

                // Binary types
                new(GenericDataTypes.Binary, "Vec<u8>", "Vec<u8>"),
                new(GenericDataTypes.VarBinary, "Vec<u8>", "Vec<u8>"),
                new(GenericDataTypes.Blob, "Vec<u8>", "Vec<u8>"),

                // Other types
                new(GenericDataTypes.Guid, "Uuid", "uuid::Uuid"),
                new(GenericDataTypes.Xml, "String", "String"),
                new(GenericDataTypes.Json, "Value", "serde_json::Value"),
            };
        }

        /// <summary>
        /// Generate Option type syntax for Rust
        /// </summary>
        public string GenerateOptionType(string typeName, bool isOptional)
        {
            if (!isOptional) return typeName;
            return $"Option<{typeName}>";
        }

        /// <summary>
        /// Generate Result type syntax for Rust
        /// </summary>
        public string GenerateResultType(string okType, string errType = "Error")
        {
            return $"Result<{okType}, {errType}>";
        }
    }
}
