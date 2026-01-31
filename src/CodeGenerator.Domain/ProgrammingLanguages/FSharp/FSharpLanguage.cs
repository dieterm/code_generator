using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.FSharp
{
    /// <summary>
    /// F# programming language definition
    /// </summary>
    public class FSharpLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<FSharpLanguage> _instance = new(() => new FSharpLanguage());
        public static FSharpLanguage Instance => _instance.Value;

        public override string Id => "fsharp";
        public override string Name => "F#";
        public override string FileExtension => ".fs";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "8.0");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private FSharpLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("8.0", "F# 8.0") { FrameworkVersion = ".NET 8", ReleaseDate = new DateTime(2023, 11, 14) },
                new("7.0", "F# 7.0") { FrameworkVersion = ".NET 7", ReleaseDate = new DateTime(2022, 11, 8) },
                new("6.0", "F# 6.0") { FrameworkVersion = ".NET 6", ReleaseDate = new DateTime(2021, 11, 8) },
                new("5.0", "F# 5.0") { FrameworkVersion = ".NET 5", ReleaseDate = new DateTime(2020, 11, 10) },
                new("4.7", "F# 4.7") { FrameworkVersion = ".NET Core 3.0", ReleaseDate = new DateTime(2019, 9, 23) },
            };

            _namingConventions = new NamingConventions.NamingConventions
            {
                ClassName = NamingStyle.PascalCase,
                InterfaceName = NamingStyle.PascalCase,
                InterfacePrefix = "I",
                MethodName = NamingStyle.CamelCase,
                PropertyName = NamingStyle.PascalCase,
                LocalVariableName = NamingStyle.CamelCase,
                ParameterName = NamingStyle.CamelCase,
                PrivateFieldName = NamingStyle.CamelCase,
                PrivateFieldPrefix = "",
                ConstantName = NamingStyle.PascalCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.PascalCase,
                NamespaceName = NamingStyle.PascalCase,
                FileName = NamingStyle.PascalCase,
                FileExtension = ".fs"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "byte", "byte"),
                new(GenericDataTypes.SmallInt, "int16", "int16"),
                new(GenericDataTypes.Int, "int", "int", "int32"),
                new(GenericDataTypes.BigInt, "int64", "int64"),

                // Decimal types
                new(GenericDataTypes.Decimal, "decimal", "decimal"),
                new(GenericDataTypes.Float, "float32", "float32", "single"),
                new(GenericDataTypes.Double, "float", "float", "double"),
                new(GenericDataTypes.Money, "decimal", "decimal"),

                // String types
                new(GenericDataTypes.Char, "char", "char"),
                new(GenericDataTypes.VarChar, "string", "string"),
                new(GenericDataTypes.NChar, "char", "char"),
                new(GenericDataTypes.NVarChar, "string", "string"),
                new(GenericDataTypes.Text, "string", "string"),
                new(GenericDataTypes.NText, "string", "string"),

                // Boolean types
                new(GenericDataTypes.Boolean, "bool", "bool"),
                new(GenericDataTypes.Bit, "bool", "bool"),

                // Date/Time types
                new(GenericDataTypes.Date, "DateOnly", "System.DateOnly"),
                new(GenericDataTypes.Time, "TimeOnly", "System.TimeOnly"),
                new(GenericDataTypes.DateTime, "DateTime", "System.DateTime"),
                new(GenericDataTypes.DateTime2, "DateTime", "System.DateTime"),
                new(GenericDataTypes.Timestamp, "DateTime", "System.DateTime"),
                new(GenericDataTypes.DateTimeOffset, "DateTimeOffset", "System.DateTimeOffset"),

                // Binary types
                new(GenericDataTypes.Binary, "byte[]", "byte[]"),
                new(GenericDataTypes.VarBinary, "byte[]", "byte[]"),
                new(GenericDataTypes.Blob, "byte[]", "byte[]"),

                // Other types
                new(GenericDataTypes.Guid, "Guid", "System.Guid"),
                new(GenericDataTypes.Xml, "string", "string"),
                new(GenericDataTypes.Json, "string", "string"),
            };
        }

        /// <summary>
        /// Generate option type syntax for F#
        /// </summary>
        public string GenerateOptionType(string typeName, bool isOptional)
        {
            if (!isOptional) return typeName;
            return $"{typeName} option";
        }
    }
}
