using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.Cpp
{
    /// <summary>
    /// C++ programming language definition
    /// </summary>
    public class CppLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<CppLanguage> _instance = new(() => new CppLanguage());
        public static CppLanguage Instance => _instance.Value;

        public override string Id => "cpp";
        public override string Name => "C++";
        public override string FileExtension => ".cpp";

        /// <summary>
        /// Header file extension
        /// </summary>
        public string HeaderFileExtension => ".h";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "C++20");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private CppLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("C++23", "C++23") { ReleaseDate = new DateTime(2023, 1, 1) },
                new("C++20", "C++20") { ReleaseDate = new DateTime(2020, 12, 1) },
                new("C++17", "C++17") { ReleaseDate = new DateTime(2017, 12, 1) },
                new("C++14", "C++14") { ReleaseDate = new DateTime(2014, 12, 1) },
                new("C++11", "C++11") { ReleaseDate = new DateTime(2011, 8, 12) },
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
                PrivateFieldPrefix = "m_",
                ConstantName = NamingStyle.UpperSnakeCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.PascalCase,
                NamespaceName = NamingStyle.SnakeCase,
                FileName = NamingStyle.PascalCase,
                FileExtension = ".cpp"
            };

            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "int8_t", "int8_t", "uint8_t"),
                new(GenericDataTypes.SmallInt, "int16_t", "int16_t"),
                new(GenericDataTypes.Int, "int32_t", "int32_t", "int"),
                new(GenericDataTypes.BigInt, "int64_t", "int64_t", "long long"),

                // Decimal types
                new(GenericDataTypes.Decimal, "double", "double"),
                new(GenericDataTypes.Float, "float", "float"),
                new(GenericDataTypes.Double, "double", "double"),
                new(GenericDataTypes.Money, "double", "double"),

                // String types
                new(GenericDataTypes.Char, "char", "char", "wchar_t"),
                new(GenericDataTypes.VarChar, "std::string", "std::string"),
                new(GenericDataTypes.NChar, "wchar_t", "wchar_t"),
                new(GenericDataTypes.NVarChar, "std::wstring", "std::wstring"),
                new(GenericDataTypes.Text, "std::string", "std::string"),
                new(GenericDataTypes.NText, "std::wstring", "std::wstring"),

                // Boolean types
                new(GenericDataTypes.Boolean, "bool", "bool"),
                new(GenericDataTypes.Bit, "bool", "bool"),

                // Date/Time types (using std::chrono)
                new(GenericDataTypes.Date, "std::chrono::year_month_day", "std::chrono::year_month_day"),
                new(GenericDataTypes.Time, "std::chrono::hh_mm_ss<std::chrono::seconds>", "std::chrono::hh_mm_ss<std::chrono::seconds>"),
                new(GenericDataTypes.DateTime, "std::chrono::system_clock::time_point", "std::chrono::system_clock::time_point"),
                new(GenericDataTypes.DateTime2, "std::chrono::system_clock::time_point", "std::chrono::system_clock::time_point"),
                new(GenericDataTypes.Timestamp, "std::chrono::system_clock::time_point", "std::chrono::system_clock::time_point"),
                new(GenericDataTypes.DateTimeOffset, "std::chrono::system_clock::time_point", "std::chrono::system_clock::time_point"),

                // Binary types
                new(GenericDataTypes.Binary, "std::vector<uint8_t>", "std::vector<uint8_t>"),
                new(GenericDataTypes.VarBinary, "std::vector<uint8_t>", "std::vector<uint8_t>"),
                new(GenericDataTypes.Blob, "std::vector<uint8_t>", "std::vector<uint8_t>"),

                // Other types
                new(GenericDataTypes.Guid, "std::string", "std::string"),
                new(GenericDataTypes.Xml, "std::string", "std::string"),
                new(GenericDataTypes.Json, "nlohmann::json", "nlohmann::json"),
            };
        }

        /// <summary>
        /// Generate optional type syntax for C++ (C++17)
        /// </summary>
        public string GenerateOptionalType(string typeName, bool isOptional)
        {
            if (!isOptional) return typeName;
            return $"std::optional<{typeName}>";
        }

        /// <summary>
        /// Generate pointer type syntax for C++
        /// </summary>
        public string GeneratePointerType(string typeName, bool isPointer)
        {
            if (!isPointer) return typeName;
            return $"{typeName}*";
        }

        /// <summary>
        /// Generate smart pointer type syntax
        /// </summary>
        public string GenerateSmartPointerType(string typeName, string pointerType = "unique")
        {
            return pointerType.ToLower() switch
            {
                "unique" => $"std::unique_ptr<{typeName}>",
                "shared" => $"std::shared_ptr<{typeName}>",
                "weak" => $"std::weak_ptr<{typeName}>",
                _ => $"std::unique_ptr<{typeName}>"
            };
        }
    }
}
