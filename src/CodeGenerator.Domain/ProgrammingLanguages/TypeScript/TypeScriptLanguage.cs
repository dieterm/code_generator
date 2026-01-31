using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.TypeScript
{
    /// <summary>
    /// TypeScript programming language definition
    /// </summary>
    public class TypeScriptLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<TypeScriptLanguage> _instance = new(() => new TypeScriptLanguage());
        public static TypeScriptLanguage Instance => _instance.Value;

        public override string Id => "typescript";
        public override string Name => "TypeScript";
        public override string FileExtension => ".ts";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "5.3");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private TypeScriptLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("5.3", "TypeScript 5.3") { ReleaseDate = new DateTime(2023, 11, 20) },
                new("5.2", "TypeScript 5.2") { ReleaseDate = new DateTime(2023, 8, 24) },
                new("5.1", "TypeScript 5.1") { ReleaseDate = new DateTime(2023, 6, 1) },
                new("5.0", "TypeScript 5.0") { ReleaseDate = new DateTime(2023, 3, 16) },
                new("4.9", "TypeScript 4.9") { ReleaseDate = new DateTime(2022, 11, 15) },
                new("4.8", "TypeScript 4.8") { ReleaseDate = new DateTime(2022, 8, 25) },
            };

            _namingConventions = new NamingConventions.NamingConventions
            {
                ClassName = NamingStyle.PascalCase,
                InterfaceName = NamingStyle.PascalCase,
                InterfacePrefix = "I",
                MethodName = NamingStyle.CamelCase,
                PropertyName = NamingStyle.CamelCase,
                LocalVariableName = NamingStyle.CamelCase,
                ParameterName = NamingStyle.CamelCase,
                PrivateFieldName = NamingStyle.CamelCase,
                PrivateFieldPrefix = "_",
                ConstantName = NamingStyle.UpperSnakeCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.PascalCase,
                NamespaceName = NamingStyle.PascalCase,
                FileName = NamingStyle.KebabCase,
                FileExtension = ".ts"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "number", "number"),
                new(GenericDataTypes.SmallInt, "number", "number"),
                new(GenericDataTypes.Int, "number", "number"),
                new(GenericDataTypes.BigInt, "bigint", "bigint"),

                // Decimal types
                new(GenericDataTypes.Decimal, "number", "number"),
                new(GenericDataTypes.Float, "number", "number"),
                new(GenericDataTypes.Double, "number", "number"),
                new(GenericDataTypes.Money, "number", "number"),

                // String types
                new(GenericDataTypes.Char, "string", "string"),
                new(GenericDataTypes.VarChar, "string", "string"),
                new(GenericDataTypes.NChar, "string", "string"),
                new(GenericDataTypes.NVarChar, "string", "string"),
                new(GenericDataTypes.Text, "string", "string"),
                new(GenericDataTypes.NText, "string", "string"),

                // Boolean types
                new(GenericDataTypes.Boolean, "boolean", "boolean"),
                new(GenericDataTypes.Bit, "boolean", "boolean"),

                // Date/Time types
                new(GenericDataTypes.Date, "Date", "Date"),
                new(GenericDataTypes.Time, "Date", "Date"),
                new(GenericDataTypes.DateTime, "Date", "Date"),
                new(GenericDataTypes.DateTime2, "Date", "Date"),
                new(GenericDataTypes.Timestamp, "Date", "Date"),
                new(GenericDataTypes.DateTimeOffset, "Date", "Date"),

                // Binary types
                new(GenericDataTypes.Binary, "ArrayBuffer", "ArrayBuffer"),
                new(GenericDataTypes.VarBinary, "ArrayBuffer", "ArrayBuffer"),
                new(GenericDataTypes.Blob, "Blob", "Blob"),

                // Other types
                new(GenericDataTypes.Guid, "string", "string"),
                new(GenericDataTypes.Xml, "string", "string"),
                new(GenericDataTypes.Json, "object", "object", "Record<string, unknown>"),
            };
        }

        /// <summary>
        /// Generate nullable/undefined type syntax for TypeScript
        /// </summary>
        public string GenerateNullableType(string typeName, bool isNullable, bool useUndefined = true)
        {
            if (!isNullable) return typeName;
            
            if (useUndefined)
                return $"{typeName} | undefined";
            else
                return $"{typeName} | null";
        }

        /// <summary>
        /// Generate optional type with both null and undefined
        /// </summary>
        public string GenerateOptionalType(string typeName, bool isOptional)
        {
            if (!isOptional) return typeName;
            return $"{typeName} | null | undefined";
        }
    }
}
