using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.Java
{
    /// <summary>
    /// Java programming language definition
    /// </summary>
    public class JavaLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<JavaLanguage> _instance = new(() => new JavaLanguage());
        public static JavaLanguage Instance => _instance.Value;

        public override string Id => "java";
        public override string Name => "Java";
        public override string FileExtension => ".java";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "21");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private JavaLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("21", "Java 21 (LTS)") { ReleaseDate = new DateTime(2023, 9, 19) },
                new("20", "Java 20") { ReleaseDate = new DateTime(2023, 3, 21) },
                new("19", "Java 19") { ReleaseDate = new DateTime(2022, 9, 20) },
                new("17", "Java 17 (LTS)") { ReleaseDate = new DateTime(2021, 9, 14) },
                new("16", "Java 16") { ReleaseDate = new DateTime(2021, 3, 16) },
                new("11", "Java 11 (LTS)") { ReleaseDate = new DateTime(2018, 9, 25) },
                new("8", "Java 8") { ReleaseDate = new DateTime(2014, 3, 18) },
            };

            _namingConventions = new NamingConventions.NamingConventions
            {
                ClassName = NamingStyle.PascalCase,
                InterfaceName = NamingStyle.PascalCase,
                InterfacePrefix = "",
                MethodName = NamingStyle.CamelCase,
                PropertyName = NamingStyle.CamelCase,
                LocalVariableName = NamingStyle.CamelCase,
                ParameterName = NamingStyle.CamelCase,
                PrivateFieldName = NamingStyle.CamelCase,
                PrivateFieldPrefix = "",
                ConstantName = NamingStyle.UpperSnakeCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.UpperSnakeCase,
                NamespaceName = NamingStyle.LowerCase,
                FileName = NamingStyle.PascalCase,
                FileExtension = ".java"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "byte", "byte"),
                new(GenericDataTypes.SmallInt, "short", "short"),
                new(GenericDataTypes.Int, "int", "int"),
                new(GenericDataTypes.BigInt, "long", "long"),

                // Decimal types
                new(GenericDataTypes.Decimal, "BigDecimal", "BigDecimal"),
                new(GenericDataTypes.Float, "float", "float"),
                new(GenericDataTypes.Double, "double", "double"),
                new(GenericDataTypes.Money, "BigDecimal", "BigDecimal"),

                // String types
                new(GenericDataTypes.Char, "String", "String"),
                new(GenericDataTypes.VarChar, "String", "String"),
                new(GenericDataTypes.NChar, "String", "String"),
                new(GenericDataTypes.NVarChar, "String", "String"),
                new(GenericDataTypes.Text, "String", "String"),
                new(GenericDataTypes.NText, "String", "String"),

                // Boolean types
                new(GenericDataTypes.Boolean, "boolean", "boolean"),
                new(GenericDataTypes.Bit, "boolean", "boolean"),

                // Date/Time types
                new(GenericDataTypes.Date, "LocalDate", "java.time.LocalDate"),
                new(GenericDataTypes.Time, "LocalTime", "java.time.LocalTime"),
                new(GenericDataTypes.DateTime, "LocalDateTime", "java.time.LocalDateTime"),
                new(GenericDataTypes.DateTime2, "LocalDateTime", "java.time.LocalDateTime"),
                new(GenericDataTypes.Timestamp, "Instant", "java.time.Instant"),
                new(GenericDataTypes.DateTimeOffset, "ZonedDateTime", "java.time.ZonedDateTime"),

                // Binary types
                new(GenericDataTypes.Binary, "byte[]", "byte[]"),
                new(GenericDataTypes.VarBinary, "byte[]", "byte[]"),
                new(GenericDataTypes.Blob, "byte[]", "byte[]"),

                // Other types
                new(GenericDataTypes.Guid, "UUID", "java.util.UUID"),
                new(GenericDataTypes.Xml, "String", "String"),
                new(GenericDataTypes.Json, "String", "String"),
            };
        }

        /// <summary>
        /// Generate nullable type syntax for Java (using Optional or @Nullable)
        /// </summary>
        public string GenerateNullableType(string typeName, bool isNullable, bool useOptional = false)
        {
            if (!isNullable) return typeName;

            if (useOptional)
            {
                // For wrapper types
                if (typeName == "int")
                    return "Optional<Integer>";
                else if (typeName == "long")
                    return "Optional<Long>";
                else if (typeName == "boolean")
                    return "Optional<Boolean>";
                else if (typeName == "double")
                    return "Optional<Double>";
                else if (typeName == "float")
                    return "Optional<Float>";
                else
                    return $"Optional<{typeName}>";
            }

            return typeName; // Reference types are nullable by default
        }
    }
}
