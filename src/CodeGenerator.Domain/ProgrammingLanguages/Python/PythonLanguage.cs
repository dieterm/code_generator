using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.Python
{
    /// <summary>
    /// Python programming language definition
    /// </summary>
    public class PythonLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<PythonLanguage> _instance = new(() => new PythonLanguage());
        public static PythonLanguage Instance => _instance.Value;

        public override string Id => "python";
        public override string Name => "Python";
        public override string FileExtension => ".py";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "3.12");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private PythonLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("3.12", "Python 3.12") { ReleaseDate = new DateTime(2023, 10, 2) },
                new("3.11", "Python 3.11") { ReleaseDate = new DateTime(2022, 10, 24) },
                new("3.10", "Python 3.10") { ReleaseDate = new DateTime(2021, 10, 4) },
                new("3.9", "Python 3.9") { ReleaseDate = new DateTime(2020, 10, 5) },
                new("3.8", "Python 3.8") { ReleaseDate = new DateTime(2019, 10, 14) },
            };

            _namingConventions = new NamingConventions.NamingConventions
            {
                ClassName = NamingStyle.PascalCase,
                InterfaceName = NamingStyle.PascalCase,
                InterfacePrefix = "",
                MethodName = NamingStyle.SnakeCase,
                PropertyName = NamingStyle.SnakeCase,
                LocalVariableName = NamingStyle.SnakeCase,
                ParameterName = NamingStyle.SnakeCase,
                PrivateFieldName = NamingStyle.SnakeCase,
                PrivateFieldPrefix = "_",
                ConstantName = NamingStyle.UpperSnakeCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.UpperSnakeCase,
                NamespaceName = NamingStyle.SnakeCase,
                FileName = NamingStyle.SnakeCase,
                FileExtension = ".py"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types (Python has arbitrary precision integers)
                new(GenericDataTypes.TinyInt, "int", "int"),
                new(GenericDataTypes.SmallInt, "int", "int"),
                new(GenericDataTypes.Int, "int", "int"),
                new(GenericDataTypes.BigInt, "int", "int"),

                // Decimal types
                new(GenericDataTypes.Decimal, "Decimal", "decimal.Decimal"),
                new(GenericDataTypes.Float, "float", "float"),
                new(GenericDataTypes.Double, "float", "float"),
                new(GenericDataTypes.Money, "Decimal", "decimal.Decimal"),

                // String types
                new(GenericDataTypes.Char, "str", "str"),
                new(GenericDataTypes.VarChar, "str", "str"),
                new(GenericDataTypes.NChar, "str", "str"),
                new(GenericDataTypes.NVarChar, "str", "str"),
                new(GenericDataTypes.Text, "str", "str"),
                new(GenericDataTypes.NText, "str", "str"),

                // Boolean types
                new(GenericDataTypes.Boolean, "bool", "bool"),
                new(GenericDataTypes.Bit, "bool", "bool"),

                // Date/Time types
                new(GenericDataTypes.Date, "date", "datetime.date"),
                new(GenericDataTypes.Time, "time", "datetime.time"),
                new(GenericDataTypes.DateTime, "datetime", "datetime.datetime"),
                new(GenericDataTypes.DateTime2, "datetime", "datetime.datetime"),
                new(GenericDataTypes.Timestamp, "datetime", "datetime.datetime"),
                new(GenericDataTypes.DateTimeOffset, "datetime", "datetime.datetime"),

                // Binary types
                new(GenericDataTypes.Binary, "bytes", "bytes"),
                new(GenericDataTypes.VarBinary, "bytes", "bytes"),
                new(GenericDataTypes.Blob, "bytes", "bytes"),

                // Other types
                new(GenericDataTypes.Guid, "UUID", "uuid.UUID"),
                new(GenericDataTypes.Xml, "str", "str"),
                new(GenericDataTypes.Json, "dict", "dict"),
            };
        }

        /// <summary>
        /// Generate Optional type hint for Python
        /// </summary>
        public string GenerateOptionalType(string typeName, bool isOptional)
        {
            if (!isOptional) return typeName;
            return $"Optional[{typeName}]";
        }

        /// <summary>
        /// Generate type hint with Union for nullable types (Python 3.10+)
        /// </summary>
        public string GenerateUnionType(string typeName, bool isOptional)
        {
            if (!isOptional) return typeName;
            return $"{typeName} | None";
        }
    }
}
