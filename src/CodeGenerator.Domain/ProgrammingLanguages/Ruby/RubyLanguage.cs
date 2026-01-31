using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages.Ruby
{
    /// <summary>
    /// Ruby programming language definition
    /// </summary>
    public class RubyLanguage : ProgrammingLanguage
    {
        private static readonly Lazy<RubyLanguage> _instance = new(() => new RubyLanguage());
        public static RubyLanguage Instance => _instance.Value;

        public override string Id => "ruby";
        public override string Name => "Ruby";
        public override string FileExtension => ".rb";

        private readonly List<ProgrammingLanguageVersion> _versions;
        public override IReadOnlyList<ProgrammingLanguageVersion> Versions => _versions;

        public override ProgrammingLanguageVersion DefaultVersion => _versions.First(v => v.Version == "3.2");

        private readonly NamingConventions.NamingConventions _namingConventions;
        public override NamingConventions.NamingConventions NamingConventions => _namingConventions;

        private readonly List<DataTypeMapping> _dataTypeMappings;
        public override IReadOnlyList<DataTypeMapping> DataTypeMappings => _dataTypeMappings;

        private RubyLanguage()
        {
            _versions = new List<ProgrammingLanguageVersion>
            {
                new("3.2", "Ruby 3.2") { ReleaseDate = new DateTime(2022, 12, 25) },
                new("3.1", "Ruby 3.1") { ReleaseDate = new DateTime(2021, 12, 25) },
                new("3.0", "Ruby 3.0") { ReleaseDate = new DateTime(2020, 12, 25) },
                new("2.7", "Ruby 2.7") { ReleaseDate = new DateTime(2019, 12, 25) },
            };

            _namingConventions = new NamingConventions.NamingConventions
            {
                ClassName = NamingStyle.PascalCase,
                InterfaceName = NamingStyle.PascalCase,  // Ruby uses modules as interfaces
                InterfacePrefix = "",
                MethodName = NamingStyle.SnakeCase,
                PropertyName = NamingStyle.SnakeCase,
                LocalVariableName = NamingStyle.SnakeCase,
                ParameterName = NamingStyle.SnakeCase,
                PrivateFieldName = NamingStyle.SnakeCase,
                PrivateFieldPrefix = "@",
                ConstantName = NamingStyle.UpperSnakeCase,
                EnumName = NamingStyle.PascalCase,
                EnumMemberName = NamingStyle.UpperSnakeCase,
                NamespaceName = NamingStyle.PascalCase,  // Modules
                FileName = NamingStyle.SnakeCase,
                FileExtension = ".rb"
            };

            // Ruby is dynamically typed
            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "Integer", "Integer"),
                new(GenericDataTypes.SmallInt, "Integer", "Integer"),
                new(GenericDataTypes.Int, "Integer", "Integer"),
                new(GenericDataTypes.BigInt, "Integer", "Integer"),

                // Decimal types
                new(GenericDataTypes.Decimal, "BigDecimal", "BigDecimal"),
                new(GenericDataTypes.Float, "Float", "Float"),
                new(GenericDataTypes.Double, "Float", "Float"),
                new(GenericDataTypes.Money, "BigDecimal", "BigDecimal"),

                // String types
                new(GenericDataTypes.Char, "String", "String"),
                new(GenericDataTypes.VarChar, "String", "String"),
                new(GenericDataTypes.NChar, "String", "String"),
                new(GenericDataTypes.NVarChar, "String", "String"),
                new(GenericDataTypes.Text, "String", "String"),
                new(GenericDataTypes.NText, "String", "String"),

                // Boolean types (Ruby uses TrueClass/FalseClass)
                new(GenericDataTypes.Boolean, "Boolean", "Boolean"),
                new(GenericDataTypes.Bit, "Boolean", "Boolean"),

                // Date/Time types
                new(GenericDataTypes.Date, "Date", "Date"),
                new(GenericDataTypes.Time, "Time", "Time"),
                new(GenericDataTypes.DateTime, "DateTime", "DateTime"),
                new(GenericDataTypes.DateTime2, "DateTime", "DateTime"),
                new(GenericDataTypes.Timestamp, "Time", "Time"),
                new(GenericDataTypes.DateTimeOffset, "DateTime", "DateTime"),

                // Binary types
                new(GenericDataTypes.Binary, "String", "String"),
                new(GenericDataTypes.VarBinary, "String", "String"),
                new(GenericDataTypes.Blob, "String", "String"),

                // Other types
                new(GenericDataTypes.Guid, "String", "String"),
                new(GenericDataTypes.Xml, "String", "String"),
                new(GenericDataTypes.Json, "Hash", "Hash"),
            };
        }
    }
}
