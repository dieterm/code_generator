using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.Kotlin
{
    /// <summary>
    /// Kotlin programming language definition
    /// </summary>
    public class KotlinLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<KotlinLanguage> _instance = new(() => new KotlinLanguage());
        public static KotlinLanguage Instance => _instance.Value;

        public override string Id => "kotlin";
        public override string Name => "Kotlin";
        public override string FileExtension => ".kt";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "1.9");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private KotlinLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("1.9", "Kotlin 1.9") { ReleaseDate = new DateTime(2023, 7, 6) },
                new("1.8", "Kotlin 1.8") { ReleaseDate = new DateTime(2023, 1, 11) },
                new("1.7", "Kotlin 1.7") { ReleaseDate = new DateTime(2022, 6, 9) },
                new("1.6", "Kotlin 1.6") { ReleaseDate = new DateTime(2021, 11, 16) },
                new("1.5", "Kotlin 1.5") { ReleaseDate = new DateTime(2021, 5, 5) },
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
                PrivateFieldPrefix = "_",
                ConstantName = NamingStyle.UpperSnakeCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.UpperSnakeCase,
                NamespaceName = NamingStyle.LowerCase,
                FileName = NamingStyle.PascalCase,
                FileExtension = ".kt"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "Byte", "Byte"),
                new(GenericDataTypes.SmallInt, "Short", "Short"),
                new(GenericDataTypes.Int, "Int", "Int"),
                new(GenericDataTypes.BigInt, "Long", "Long"),

                // Decimal types
                new(GenericDataTypes.Decimal, "BigDecimal", "java.math.BigDecimal"),
                new(GenericDataTypes.Float, "Float", "Float"),
                new(GenericDataTypes.Double, "Double", "Double"),
                new(GenericDataTypes.Money, "BigDecimal", "java.math.BigDecimal"),

                // String types
                new(GenericDataTypes.Char, "Char", "Char"),
                new(GenericDataTypes.VarChar, "String", "String"),
                new(GenericDataTypes.NChar, "Char", "Char"),
                new(GenericDataTypes.NVarChar, "String", "String"),
                new(GenericDataTypes.Text, "String", "String"),
                new(GenericDataTypes.NText, "String", "String"),

                // Boolean types
                new(GenericDataTypes.Boolean, "Boolean", "Boolean"),
                new(GenericDataTypes.Bit, "Boolean", "Boolean"),

                // Date/Time types
                new(GenericDataTypes.Date, "LocalDate", "java.time.LocalDate"),
                new(GenericDataTypes.Time, "LocalTime", "java.time.LocalTime"),
                new(GenericDataTypes.DateTime, "LocalDateTime", "java.time.LocalDateTime"),
                new(GenericDataTypes.DateTime2, "LocalDateTime", "java.time.LocalDateTime"),
                new(GenericDataTypes.Timestamp, "Instant", "java.time.Instant"),
                new(GenericDataTypes.DateTimeOffset, "ZonedDateTime", "java.time.ZonedDateTime"),

                // Binary types
                new(GenericDataTypes.Binary, "ByteArray", "ByteArray"),
                new(GenericDataTypes.VarBinary, "ByteArray", "ByteArray"),
                new(GenericDataTypes.Blob, "ByteArray", "ByteArray"),

                // Other types
                new(GenericDataTypes.Guid, "UUID", "java.util.UUID"),
                new(GenericDataTypes.Xml, "String", "String"),
                new(GenericDataTypes.Json, "String", "String"),
            };
        }

        /// <summary>
        /// Generate nullable type syntax for Kotlin
        /// </summary>
        public string GenerateNullableType(string typeName, bool isNullable)
        {
            if (!isNullable) return typeName;
            return $"{typeName}?";
        }
    }
}
