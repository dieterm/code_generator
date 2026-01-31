using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.Swift
{
    /// <summary>
    /// Swift programming language definition
    /// </summary>
    public class SwiftLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<SwiftLanguage> _instance = new(() => new SwiftLanguage());
        public static SwiftLanguage Instance => _instance.Value;

        public override string Id => "swift";
        public override string Name => "Swift";
        public override string FileExtension => ".swift";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "5.9");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private SwiftLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("5.9", "Swift 5.9") { ReleaseDate = new DateTime(2023, 9, 18) },
                new("5.8", "Swift 5.8") { ReleaseDate = new DateTime(2023, 3, 30) },
                new("5.7", "Swift 5.7") { ReleaseDate = new DateTime(2022, 9, 12) },
                new("5.6", "Swift 5.6") { ReleaseDate = new DateTime(2022, 3, 14) },
                new("5.5", "Swift 5.5") { ReleaseDate = new DateTime(2021, 9, 20) },
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
                ConstantName = NamingStyle.CamelCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.CamelCase,
                NamespaceName = NamingStyle.PascalCase,
                FileName = NamingStyle.PascalCase,
                FileExtension = ".swift"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "Int8", "Int8", "UInt8"),
                new(GenericDataTypes.SmallInt, "Int16", "Int16"),
                new(GenericDataTypes.Int, "Int", "Int", "Int32"),
                new(GenericDataTypes.BigInt, "Int64", "Int64"),

                // Decimal types
                new(GenericDataTypes.Decimal, "Decimal", "Decimal"),
                new(GenericDataTypes.Float, "Float", "Float"),
                new(GenericDataTypes.Double, "Double", "Double"),
                new(GenericDataTypes.Money, "Decimal", "Decimal"),

                // String types
                new(GenericDataTypes.Char, "Character", "Character"),
                new(GenericDataTypes.VarChar, "String", "String"),
                new(GenericDataTypes.NChar, "Character", "Character"),
                new(GenericDataTypes.NVarChar, "String", "String"),
                new(GenericDataTypes.Text, "String", "String"),
                new(GenericDataTypes.NText, "String", "String"),

                // Boolean types
                new(GenericDataTypes.Boolean, "Bool", "Bool"),
                new(GenericDataTypes.Bit, "Bool", "Bool"),

                // Date/Time types
                new(GenericDataTypes.Date, "Date", "Foundation.Date"),
                new(GenericDataTypes.Time, "Date", "Foundation.Date"),
                new(GenericDataTypes.DateTime, "Date", "Foundation.Date"),
                new(GenericDataTypes.DateTime2, "Date", "Foundation.Date"),
                new(GenericDataTypes.Timestamp, "Date", "Foundation.Date"),
                new(GenericDataTypes.DateTimeOffset, "Date", "Foundation.Date"),

                // Binary types
                new(GenericDataTypes.Binary, "Data", "Foundation.Data"),
                new(GenericDataTypes.VarBinary, "Data", "Foundation.Data"),
                new(GenericDataTypes.Blob, "Data", "Foundation.Data"),

                // Other types
                new(GenericDataTypes.Guid, "UUID", "Foundation.UUID"),
                new(GenericDataTypes.Xml, "String", "String"),
                new(GenericDataTypes.Json, "Any", "Any"),
            };
        }

        /// <summary>
        /// Generate optional type syntax for Swift
        /// </summary>
        public string GenerateOptionalType(string typeName, bool isOptional)
        {
            if (!isOptional) return typeName;
            return $"{typeName}?";
        }

        /// <summary>
        /// Generate implicitly unwrapped optional type
        /// </summary>
        public string GenerateImplicitlyUnwrappedOptionalType(string typeName)
        {
            return $"{typeName}!";
        }
    }
}
