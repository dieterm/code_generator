using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.Php
{
    /// <summary>
    /// PHP programming language definition
    /// </summary>
    public class PhpLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<PhpLanguage> _instance = new(() => new PhpLanguage());
        public static PhpLanguage Instance => _instance.Value;

        public override string Id => "php";
        public override string Name => "PHP";
        public override string FileExtension => ".php";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "8.3");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private PhpLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("8.3", "PHP 8.3") { ReleaseDate = new DateTime(2023, 11, 23) },
                new("8.2", "PHP 8.2") { ReleaseDate = new DateTime(2022, 12, 8) },
                new("8.1", "PHP 8.1") { ReleaseDate = new DateTime(2021, 11, 25) },
                new("8.0", "PHP 8.0") { ReleaseDate = new DateTime(2020, 11, 26) },
                new("7.4", "PHP 7.4") { ReleaseDate = new DateTime(2019, 11, 28) },
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
                EnumMemberName = NamingStyle.PascalCase,
                NamespaceName = NamingStyle.PascalCase,
                FileName = NamingStyle.PascalCase,
                FileExtension = ".php"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "int", "int"),
                new(GenericDataTypes.SmallInt, "int", "int"),
                new(GenericDataTypes.Int, "int", "int"),
                new(GenericDataTypes.BigInt, "int", "int"),

                // Decimal types
                new(GenericDataTypes.Decimal, "float", "float"),
                new(GenericDataTypes.Float, "float", "float"),
                new(GenericDataTypes.Double, "float", "float"),
                new(GenericDataTypes.Money, "float", "float"),

                // String types
                new(GenericDataTypes.Char, "string", "string"),
                new(GenericDataTypes.VarChar, "string", "string"),
                new(GenericDataTypes.NChar, "string", "string"),
                new(GenericDataTypes.NVarChar, "string", "string"),
                new(GenericDataTypes.Text, "string", "string"),
                new(GenericDataTypes.NText, "string", "string"),

                // Boolean types
                new(GenericDataTypes.Boolean, "bool", "bool"),
                new(GenericDataTypes.Bit, "bool", "bool"),

                // Date/Time types
                new(GenericDataTypes.Date, "DateTimeInterface", "\\DateTimeInterface"),
                new(GenericDataTypes.Time, "DateTimeInterface", "\\DateTimeInterface"),
                new(GenericDataTypes.DateTime, "DateTimeInterface", "\\DateTimeInterface"),
                new(GenericDataTypes.DateTime2, "DateTimeInterface", "\\DateTimeInterface"),
                new(GenericDataTypes.Timestamp, "DateTimeInterface", "\\DateTimeInterface"),
                new(GenericDataTypes.DateTimeOffset, "DateTimeInterface", "\\DateTimeInterface"),

                // Binary types
                new(GenericDataTypes.Binary, "string", "string"),
                new(GenericDataTypes.VarBinary, "string", "string"),
                new(GenericDataTypes.Blob, "string", "string"),

                // Other types
                new(GenericDataTypes.Guid, "string", "string"),
                new(GenericDataTypes.Xml, "string", "string"),
                new(GenericDataTypes.Json, "array", "array"),
            };
        }

        /// <summary>
        /// Generate nullable type syntax for PHP 8+
        /// </summary>
        public string GenerateNullableType(string typeName, bool isNullable)
        {
            if (!isNullable) return typeName;
            return $"?{typeName}";
        }

        /// <summary>
        /// Generate union type syntax for PHP 8+
        /// </summary>
        public string GenerateUnionType(params string[] types)
        {
            return string.Join("|", types);
        }
    }
}
