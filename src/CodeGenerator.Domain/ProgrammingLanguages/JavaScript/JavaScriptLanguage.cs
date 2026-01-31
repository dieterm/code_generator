using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.JavaScript
{
    /// <summary>
    /// JavaScript programming language definition
    /// </summary>
    public class JavaScriptLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<JavaScriptLanguage> _instance = new(() => new JavaScriptLanguage());
        public static JavaScriptLanguage Instance => _instance.Value;

        public override string Id => "javascript";
        public override string Name => "JavaScript";
        public override string FileExtension => ".js";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "ES2023");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private JavaScriptLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("ES2023", "ECMAScript 2023") { ReleaseDate = new DateTime(2023, 6, 1) },
                new("ES2022", "ECMAScript 2022") { ReleaseDate = new DateTime(2022, 6, 1) },
                new("ES2021", "ECMAScript 2021") { ReleaseDate = new DateTime(2021, 6, 1) },
                new("ES2020", "ECMAScript 2020") { ReleaseDate = new DateTime(2020, 6, 1) },
                new("ES2019", "ECMAScript 2019") { ReleaseDate = new DateTime(2019, 6, 1) },
                new("ES2018", "ECMAScript 2018") { ReleaseDate = new DateTime(2018, 6, 1) },
                new("ES6", "ECMAScript 2015 (ES6)") { ReleaseDate = new DateTime(2015, 6, 1) },
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
                NamespaceName = NamingStyle.CamelCase,
                FileName = NamingStyle.KebabCase,
                FileExtension = ".js"
            };

            // JavaScript is dynamically typed, so all types map to their JS equivalents
            _dataTypeMappings = new List<DataTypeMapping>
            {
                // All numbers are 'number' in JavaScript
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
                new(GenericDataTypes.Json, "object", "object"),
            };
        }
    }
}
