using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.VisualBasic
{
    /// <summary>
    /// Visual Basic .NET programming language definition
    /// </summary>
    public class VisualBasicLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<VisualBasicLanguage> _instance = new(() => new VisualBasicLanguage());
        public static VisualBasicLanguage Instance => _instance.Value;

        public override string Id => "vb";
        public override string Name => "Visual Basic";
        public override string FileExtension => ".vb";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "16.9");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private VisualBasicLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("16.9", "VB 16.9") { FrameworkVersion = ".NET 8", ReleaseDate = new DateTime(2023, 11, 14) },
                new("16.0", "VB 16.0") { FrameworkVersion = ".NET 5", ReleaseDate = new DateTime(2020, 11, 10) },
                new("15.5", "VB 15.5") { FrameworkVersion = ".NET Framework 4.7.2", ReleaseDate = new DateTime(2017, 12, 4) },
                new("15.0", "VB 15.0") { FrameworkVersion = ".NET Framework 4.6.2", ReleaseDate = new DateTime(2017, 3, 7) },
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
                FileExtension = ".vb"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "Byte", "Byte"),
                new(GenericDataTypes.SmallInt, "Short", "Short", "Int16"),
                new(GenericDataTypes.Int, "Integer", "Integer", "Int32"),
                new(GenericDataTypes.BigInt, "Long", "Long", "Int64"),

                // Decimal types
                new(GenericDataTypes.Decimal, "Decimal", "Decimal"),
                new(GenericDataTypes.Float, "Single", "Single"),
                new(GenericDataTypes.Double, "Double", "Double"),
                new(GenericDataTypes.Money, "Decimal", "Decimal"),

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
                new(GenericDataTypes.Date, "Date", "Date"),
                new(GenericDataTypes.Time, "TimeSpan", "TimeSpan"),
                new(GenericDataTypes.DateTime, "DateTime", "DateTime"),
                new(GenericDataTypes.DateTime2, "DateTime", "DateTime"),
                new(GenericDataTypes.Timestamp, "DateTime", "DateTime"),
                new(GenericDataTypes.DateTimeOffset, "DateTimeOffset", "DateTimeOffset"),

                // Binary types
                new(GenericDataTypes.Binary, "Byte()", "Byte()"),
                new(GenericDataTypes.VarBinary, "Byte()", "Byte()"),
                new(GenericDataTypes.Blob, "Byte()", "Byte()"),

                // Other types
                new(GenericDataTypes.Guid, "Guid", "Guid"),
                new(GenericDataTypes.Xml, "String", "String"),
                new(GenericDataTypes.Json, "String", "String"),
            };
        }

        /// <summary>
        /// Generate nullable type syntax for VB.NET
        /// </summary>
        public string GenerateNullableType(string typeName, bool isNullable)
        {
            if (!isNullable) return typeName;

            var valueTypes = new[] { "Byte", "SByte", "Short", "UShort", "Integer", "UInteger", "Long", "ULong",
                "Single", "Double", "Decimal", "Boolean", "Char", "DateTime", "Date", "TimeSpan",
                "DateTimeOffset", "Guid" };

            if (valueTypes.Contains(typeName))
            {
                return typeName + "?";
            }

            return typeName;
        }
    }
}
