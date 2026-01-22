namespace CodeGenerator.Domain.DataTypes
{
    /// <summary>
    /// Represents a mapping from a generic data type to a specific implementation
    /// in a database or programming language
    /// </summary>
    public class DataTypeMapping
    {
        /// <summary>
        /// The generic data type being mapped
        /// </summary>
        public GenericDataType GenericType { get; }

        /// <summary>
        /// The native type name (e.g., "NVARCHAR", "string", "int")
        /// </summary>
        public string NativeTypeName { get; }

        /// <summary>
        /// Format string for generating the type definition
        /// Placeholders: {maxlength}, {precision}, {scale}
        /// Examples: "NVARCHAR({maxlength})", "decimal({precision},{scale})"
        /// </summary>
        public string FormatString { get; }

        /// <summary>
        /// Aliases that map to this type (for parsing)
        /// </summary>
        public IReadOnlyList<string> Aliases { get; }

        /// <summary>
        /// Minimum max length value (if applicable)
        /// </summary>
        public int? MinMaxLength { get; set; }

        /// <summary>
        /// Maximum max length value (if applicable)
        /// -1 typically means "MAX" or unlimited
        /// </summary>
        public int? MaxMaxLength { get; set; }

        /// <summary>
        /// Special value for unlimited length (e.g., "MAX" for SQL Server)
        /// </summary>
        public string? UnlimitedLengthKeyword { get; set; }

        /// <summary>
        /// Maximum precision value (if applicable)
        /// </summary>
        public int? MaxPrecision { get; set; }

        /// <summary>
        /// Maximum scale value (if applicable)
        /// </summary>
        public int? MaxScale { get; set; }

        /// <summary>
        /// Notes or documentation about this mapping
        /// </summary>
        public string? Notes { get; set; }

        public DataTypeMapping(
            GenericDataType genericType,
            string nativeTypeName,
            string formatString,
            params string[] aliases)
        {
            GenericType = genericType;
            NativeTypeName = nativeTypeName;
            FormatString = formatString;
            Aliases = aliases.ToList();
        }

        /// <summary>
        /// Generate the type definition string
        /// </summary>
        public string GenerateTypeDef(int? maxLength = null, int? precision = null, int? scale = null)
        {
            var result = FormatString;

            if (maxLength.HasValue)
            {
                var lengthStr = (maxLength == -1 && !string.IsNullOrEmpty(UnlimitedLengthKeyword))
                    ? UnlimitedLengthKeyword
                    : maxLength.Value.ToString();
                result = result.Replace("{maxlength}", lengthStr);
            }

            if (precision.HasValue)
            {
                result = result.Replace("{precision}", precision.Value.ToString());
            }

            if (scale.HasValue)
            {
                result = result.Replace("{scale}", scale.Value.ToString());
            }

            // Remove any unused placeholders and their surrounding parentheses
            if (!maxLength.HasValue && result.Contains("{maxlength}"))
            {
                result = result.Replace("({maxlength})", "");
                result = result.Replace("{maxlength}", "");
            }

            if (!precision.HasValue && result.Contains("{precision}"))
            {
                // Remove precision/scale portion
                var start = result.IndexOf('(');
                var end = result.IndexOf(')');
                if (start >= 0 && end > start)
                {
                    result = result.Substring(0, start);
                }
            }

            return result.Trim();
        }

        /// <summary>
        /// Check if a type name matches this mapping
        /// </summary>
        public bool Matches(string typeName)
        {
            var cleanType = ExtractBaseTypeName(typeName);
            
            if (NativeTypeName.Equals(cleanType, StringComparison.OrdinalIgnoreCase))
                return true;

            return Aliases.Any(a => a.Equals(cleanType, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Extract the base type name from a full type definition
        /// e.g., "varchar(255)" -> "varchar"
        /// </summary>
        private static string ExtractBaseTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return typeName;

            var parenIndex = typeName.IndexOf('(');
            if (parenIndex > 0)
            {
                return typeName.Substring(0, parenIndex).Trim();
            }

            var spaceIndex = typeName.IndexOf(' ');
            if (spaceIndex > 0)
            {
                return typeName.Substring(0, spaceIndex).Trim();
            }

            return typeName.Trim();
        }
    }
}
