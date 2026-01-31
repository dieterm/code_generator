using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.Go
{
    /// <summary>
    /// Go programming language definition
    /// </summary>
    public class GoLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<GoLanguage> _instance = new(() => new GoLanguage());
        public static GoLanguage Instance => _instance.Value;

        public override string Id => "go";
        public override string Name => "Go";
        public override string FileExtension => ".go";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "1.21");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private GoLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("1.21", "Go 1.21") { ReleaseDate = new DateTime(2023, 8, 8) },
                new("1.20", "Go 1.20") { ReleaseDate = new DateTime(2023, 2, 1) },
                new("1.19", "Go 1.19") { ReleaseDate = new DateTime(2022, 8, 2) },
                new("1.18", "Go 1.18") { ReleaseDate = new DateTime(2022, 3, 15) },
                new("1.17", "Go 1.17") { ReleaseDate = new DateTime(2021, 8, 16) },
            };

            // Go uses visibility based on capitalization: PascalCase = exported, camelCase = unexported
            _namingConventions = new NamingConventions.NamingConventions
            {
                ClassName = NamingStyle.PascalCase,          // Exported struct
                InterfaceName = NamingStyle.PascalCase,
                InterfacePrefix = "",
                MethodName = NamingStyle.PascalCase,         // Exported method
                PropertyName = NamingStyle.PascalCase,       // Exported field
                LocalVariableName = NamingStyle.CamelCase,
                ParameterName = NamingStyle.CamelCase,
                PrivateFieldName = NamingStyle.CamelCase,    // Unexported field
                PrivateFieldPrefix = "",
                ConstantName = NamingStyle.PascalCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.PascalCase,
                NamespaceName = NamingStyle.LowerCase,       // Package names
                FileName = NamingStyle.SnakeCase,
                FileExtension = ".go"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "int8", "int8", "uint8", "byte"),
                new(GenericDataTypes.SmallInt, "int16", "int16"),
                new(GenericDataTypes.Int, "int", "int", "int32"),
                new(GenericDataTypes.BigInt, "int64", "int64"),

                // Decimal types (Go doesn't have built-in decimal, use packages)
                new(GenericDataTypes.Decimal, "float64", "float64"),
                new(GenericDataTypes.Float, "float32", "float32"),
                new(GenericDataTypes.Double, "float64", "float64"),
                new(GenericDataTypes.Money, "float64", "float64"),

                // String types
                new(GenericDataTypes.Char, "rune", "rune"),
                new(GenericDataTypes.VarChar, "string", "string"),
                new(GenericDataTypes.NChar, "rune", "rune"),
                new(GenericDataTypes.NVarChar, "string", "string"),
                new(GenericDataTypes.Text, "string", "string"),
                new(GenericDataTypes.NText, "string", "string"),

                // Boolean types
                new(GenericDataTypes.Boolean, "bool", "bool"),
                new(GenericDataTypes.Bit, "bool", "bool"),

                // Date/Time types
                new(GenericDataTypes.Date, "time.Time", "time.Time"),
                new(GenericDataTypes.Time, "time.Time", "time.Time"),
                new(GenericDataTypes.DateTime, "time.Time", "time.Time"),
                new(GenericDataTypes.DateTime2, "time.Time", "time.Time"),
                new(GenericDataTypes.Timestamp, "time.Time", "time.Time"),
                new(GenericDataTypes.DateTimeOffset, "time.Time", "time.Time"),

                // Binary types
                new(GenericDataTypes.Binary, "[]byte", "[]byte"),
                new(GenericDataTypes.VarBinary, "[]byte", "[]byte"),
                new(GenericDataTypes.Blob, "[]byte", "[]byte"),

                // Other types
                new(GenericDataTypes.Guid, "uuid.UUID", "github.com/google/uuid.UUID"),
                new(GenericDataTypes.Xml, "string", "string"),
                new(GenericDataTypes.Json, "interface{}", "interface{}", "any"),
            };
        }

        /// <summary>
        /// Generate pointer type syntax for Go (for nullable types)
        /// </summary>
        public string GeneratePointerType(string typeName, bool isPointer)
        {
            if (!isPointer) return typeName;
            return $"*{typeName}";
        }
    }
}
