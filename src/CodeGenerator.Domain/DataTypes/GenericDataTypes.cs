namespace CodeGenerator.Domain.DataTypes
{
    /// <summary>
    /// Static registry of all generic data types
    /// </summary>
    public static class GenericDataTypes
    {
        // Integer types
        public static readonly GenericDataType TinyInt = new("tinyint", "Tiny Integer", DataTypeCategory.Integer)
        {
            Description = "8-bit integer (0 to 255 or -128 to 127)"
        };

        public static readonly GenericDataType SmallInt = new("smallint", "Small Integer", DataTypeCategory.Integer)
        {
            Description = "16-bit integer (-32,768 to 32,767)"
        };

        public static readonly GenericDataType Int = new("int", "Integer", DataTypeCategory.Integer)
        {
            Description = "32-bit integer (-2,147,483,648 to 2,147,483,647)"
        };

        public static readonly GenericDataType BigInt = new("bigint", "Big Integer", DataTypeCategory.Integer)
        {
            Description = "64-bit integer"
        };

        // Decimal types
        public static readonly GenericDataType Decimal = new("decimal", "Decimal", DataTypeCategory.Decimal)
        {
            Description = "Fixed precision and scale numeric",
            SupportsPrecision = true,
            SupportsScale = true,
            DefaultPrecision = 18,
            DefaultScale = 2
        };

        public static readonly GenericDataType Float = new("float", "Float", DataTypeCategory.Decimal)
        {
            Description = "Approximate floating-point number"
        };

        public static readonly GenericDataType Double = new("double", "Double", DataTypeCategory.Decimal)
        {
            Description = "Double precision floating-point number"
        };

        public static readonly GenericDataType Money = new("money", "Money", DataTypeCategory.Decimal)
        {
            Description = "Currency value"
        };

        // String types
        public static readonly GenericDataType Char = new("char", "Fixed Character", DataTypeCategory.String)
        {
            Description = "Fixed-length character string",
            SupportsMaxLength = true,
            DefaultMaxLength = 1
        };

        public static readonly GenericDataType VarChar = new("varchar", "Variable Character", DataTypeCategory.String)
        {
            Description = "Variable-length character string",
            SupportsMaxLength = true,
            DefaultMaxLength = 255
        };

        public static readonly GenericDataType NChar = new("nchar", "Fixed Unicode Character", DataTypeCategory.String)
        {
            Description = "Fixed-length Unicode character string",
            SupportsMaxLength = true,
            DefaultMaxLength = 1
        };

        public static readonly GenericDataType NVarChar = new("nvarchar", "Variable Unicode Character", DataTypeCategory.String)
        {
            Description = "Variable-length Unicode character string",
            SupportsMaxLength = true,
            DefaultMaxLength = 255
        };

        public static readonly GenericDataType Text = new("text", "Text", DataTypeCategory.String)
        {
            Description = "Large text data"
        };

        public static readonly GenericDataType NText = new("ntext", "Unicode Text", DataTypeCategory.String)
        {
            Description = "Large Unicode text data"
        };

        // Enum types
        public static readonly GenericDataType Enum = new("enum", "Enumeration", DataTypeCategory.Enum)
        {
            Description = "Enumerated type with predefined set of values",
            SupportsAllowedValues = true
        };

        // Boolean
        public static readonly GenericDataType Boolean = new("boolean", "Boolean", DataTypeCategory.Boolean)
        {
            Description = "True/False value"
        };

        public static readonly GenericDataType Bit = new("bit", "Bit", DataTypeCategory.Boolean)
        {
            Description = "Single bit value (0 or 1)"
        };

        // Date/Time types
        public static readonly GenericDataType Date = new("date", "Date", DataTypeCategory.DateTime)
        {
            Description = "Date only (no time component)"
        };

        public static readonly GenericDataType Time = new("time", "Time", DataTypeCategory.DateTime)
        {
            Description = "Time only (no date component)"
        };

        public static readonly GenericDataType DateTime = new("datetime", "Date and Time", DataTypeCategory.DateTime)
        {
            Description = "Date and time"
        };

        public static readonly GenericDataType DateTime2 = new("datetime2", "Date and Time 2", DataTypeCategory.DateTime)
        {
            Description = "Date and time with higher precision"
        };

        public static readonly GenericDataType Timestamp = new("timestamp", "Timestamp", DataTypeCategory.DateTime)
        {
            Description = "Automatic timestamp"
        };

        public static readonly GenericDataType DateTimeOffset = new("datetimeoffset", "Date Time with Offset", DataTypeCategory.DateTime)
        {
            Description = "Date and time with timezone offset"
        };

        // Binary types
        public static readonly GenericDataType Binary = new("binary", "Fixed Binary", DataTypeCategory.Binary)
        {
            Description = "Fixed-length binary data",
            SupportsMaxLength = true,
            DefaultMaxLength = 50
        };

        public static readonly GenericDataType VarBinary = new("varbinary", "Variable Binary", DataTypeCategory.Binary)
        {
            Description = "Variable-length binary data",
            SupportsMaxLength = true,
            DefaultMaxLength = 50
        };

        public static readonly GenericDataType Blob = new("blob", "Binary Large Object", DataTypeCategory.Binary)
        {
            Description = "Large binary data"
        };

        // Other types
        public static readonly GenericDataType Guid = new("guid", "GUID/UUID", DataTypeCategory.Guid)
        {
            Description = "Globally unique identifier"
        };

        public static readonly GenericDataType Xml = new("xml", "XML", DataTypeCategory.Other)
        {
            Description = "XML data"
        };

        public static readonly GenericDataType Json = new("json", "JSON", DataTypeCategory.Other)
        {
            Description = "JSON data"
        };

        /// <summary>
        /// Get all generic data types
        /// </summary>
        public static IEnumerable<GenericDataType> All => new[]
        {
            TinyInt, SmallInt, Int, BigInt,
            Decimal, Float, Double, Money,
            Char, VarChar, NChar, NVarChar, Text, NText,
            Enum,
            Boolean, Bit,
            Date, Time, DateTime, DateTime2, Timestamp, DateTimeOffset,
            Binary, VarBinary, Blob,
            Guid, Xml, Json
        };

        public static IEnumerable<GenericDataType> NumericTypes => new[]
        {
            TinyInt, SmallInt, Int, BigInt,
            Decimal, Float, Double, Money
        };

        public static IEnumerable<GenericDataType> TextBasedTypes => new[]
        {
            Char, VarChar, NChar, NVarChar, Text, NText
        };

        public static IEnumerable<GenericDataType> BooleanTypes => new[]
        {
            Boolean, Bit
        };

        public static IEnumerable<GenericDataType> DateTypes => new[]
        {
            Date, Time, DateTime, DateTime2, Timestamp, DateTimeOffset
        };

        public static IEnumerable<GenericDataType> BinaryTypes => new[]
        {
            Binary, VarBinary, Blob
        };

        public static IEnumerable<GenericDataType> EnumTypes => new[]
        {
            Enum
        };

        /// <summary>
        /// Find a generic data type by id
        /// </summary>
        public static GenericDataType? FindById(string id)
        {
            return All.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
        
        public static bool IsBinaryType(string dataType)
        {
            return BinaryTypes.Any(t => t.Id.Equals(dataType, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsDateType(string dataType)
        {
            return DateTypes.Any(t => t.Id.Equals(dataType, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsNumericType(string dataType)
        {
            return NumericTypes.Any(t => t.Id.Equals(dataType, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsTextBasedType(string dataType)
        {
            return TextBasedTypes.Any(t => t.Id.Equals(dataType, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsBooleanType(string dataType)
        {
            return BooleanTypes.Any(t => t.Id.Equals(dataType, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsEnumType(string dataType)
        {
            return EnumTypes.Any(t => t.Id.Equals(dataType, StringComparison.OrdinalIgnoreCase));
        }
    }
}
