using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages
{
    /// <summary>
    /// C# programming language definition
    /// </summary>
    public class CSharpLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<CSharpLanguage> _instance = new(() => new CSharpLanguage());
        public static CSharpLanguage Instance => _instance.Value;

        public override string Id => "csharp";
        public override string Name => "C#";
        public override string FileExtension => ".cs";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "12.0");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private CSharpLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("12.0", "C# 12") { FrameworkVersion = ".NET 8", ReleaseDate = new DateTime(2023, 11, 14) },
                new("11.0", "C# 11") { FrameworkVersion = ".NET 7", ReleaseDate = new DateTime(2022, 11, 8) },
                new("10.0", "C# 10") { FrameworkVersion = ".NET 6", ReleaseDate = new DateTime(2021, 11, 8) },
                new("9.0", "C# 9") { FrameworkVersion = ".NET 5", ReleaseDate = new DateTime(2020, 11, 10) },
                new("8.0", "C# 8") { FrameworkVersion = ".NET Core 3.0", ReleaseDate = new DateTime(2019, 9, 23) },
                new("7.3", "C# 7.3") { FrameworkVersion = ".NET Framework 4.7.2", ReleaseDate = new DateTime(2018, 5, 30) },
            };

            _namingConventions = new NamingConventions.NamingConventions
            {
                ClassName = NamingStyle.PascalCase,
                InterfaceName = NamingStyle.PascalCase,
                InterfacePrefix = "I",
                MethodName = NamingStyle.PascalCase,
                PropertyName = NamingStyle.PascalCase,
                LocalVariableName = NamingStyle.CamelCase,
                ParameterName = NamingStyle.CamelCase,
                PrivateFieldName = NamingStyle.CamelCase,
                PrivateFieldPrefix = "_",
                ConstantName = NamingStyle.PascalCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.PascalCase,
                NamespaceName = NamingStyle.PascalCase,
                FileName = NamingStyle.PascalCase,
                FileExtension = ".cs"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "byte", "byte", "sbyte"),
                new(GenericDataTypes.SmallInt, "short", "short", "Int16"),
                new(GenericDataTypes.Int, "int", "int", "Int32"),
                new(GenericDataTypes.BigInt, "long", "long", "Int64"),

                // Decimal types
                new(GenericDataTypes.Decimal, "decimal", "decimal"),
                new(GenericDataTypes.Float, "float", "float", "Single"),
                new(GenericDataTypes.Double, "double", "double", "Double"),
                new(GenericDataTypes.Money, "decimal", "decimal"),

                // String types
                new(GenericDataTypes.Char, "char", "char"),
                new(GenericDataTypes.VarChar, "string", "string", "String"),
                new(GenericDataTypes.NChar, "char", "char"),
                new(GenericDataTypes.NVarChar, "string", "string", "String"),
                new(GenericDataTypes.Text, "string", "string"),
                new(GenericDataTypes.NText, "string", "string"),

                // Boolean types
                new(GenericDataTypes.Boolean, "bool", "bool", "Boolean"),
                new(GenericDataTypes.Bit, "bool", "bool"),

                // Date/Time types
                new(GenericDataTypes.Date, "DateOnly", "DateOnly"),
                new(GenericDataTypes.Time, "TimeOnly", "TimeOnly"),
                new(GenericDataTypes.DateTime, "DateTime", "DateTime"),
                new(GenericDataTypes.DateTime2, "DateTime", "DateTime"),
                new(GenericDataTypes.Timestamp, "DateTime", "DateTime"),
                new(GenericDataTypes.DateTimeOffset, "DateTimeOffset", "DateTimeOffset"),

                // Binary types
                new(GenericDataTypes.Binary, "byte[]", "byte[]"),
                new(GenericDataTypes.VarBinary, "byte[]", "byte[]"),
                new(GenericDataTypes.Blob, "byte[]", "byte[]"),

                // Other types
                new(GenericDataTypes.Guid, "Guid", "Guid"),
                new(GenericDataTypes.Xml, "string", "string"),
                new(GenericDataTypes.Json, "string", "string"),
            };
        }

        /// <summary>
        /// Generate nullable type syntax
        /// </summary>
        public string GenerateNullableType(string typeName, bool isNullable)
        {
            if (!isNullable) return typeName;

            // Reference types are already nullable with nullable reference types enabled
            var valueTypes = new[] { "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong",
                "float", "double", "decimal", "bool", "char", "DateTime", "DateOnly", "TimeOnly",
                "DateTimeOffset", "Guid" };

            if (valueTypes.Contains(typeName))
            {
                return typeName + "?";
            }

            return typeName;
        }
    }
}
