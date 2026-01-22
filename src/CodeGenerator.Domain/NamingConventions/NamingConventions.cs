namespace CodeGenerator.Domain.NamingConventions
{

    /// <summary>
    /// Defines naming conventions for different code elements
    /// </summary>
    public class NamingConventions
    {
        /// <summary>
        /// Convention for class names
        /// </summary>
        public NamingStyle ClassName { get; set; } = NamingStyle.PascalCase;

        /// <summary>
        /// Convention for interface names
        /// </summary>
        public NamingStyle InterfaceName { get; set; } = NamingStyle.PascalCase;

        /// <summary>
        /// Prefix for interface names (e.g., "I" for C#)
        /// </summary>
        public string InterfacePrefix { get; set; } = string.Empty;

        /// <summary>
        /// Convention for method names
        /// </summary>
        public NamingStyle MethodName { get; set; } = NamingStyle.PascalCase;

        /// <summary>
        /// Convention for property names
        /// </summary>
        public NamingStyle PropertyName { get; set; } = NamingStyle.PascalCase;

        /// <summary>
        /// Convention for local variable names
        /// </summary>
        public NamingStyle LocalVariableName { get; set; } = NamingStyle.CamelCase;

        /// <summary>
        /// Convention for parameter names
        /// </summary>
        public NamingStyle ParameterName { get; set; } = NamingStyle.CamelCase;

        /// <summary>
        /// Convention for private field names
        /// </summary>
        public NamingStyle PrivateFieldName { get; set; } = NamingStyle.CamelCase;

        /// <summary>
        /// Prefix for private fields (e.g., "_" for C#)
        /// </summary>
        public string PrivateFieldPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Convention for constant names
        /// </summary>
        public NamingStyle ConstantName { get; set; } = NamingStyle.PascalCase;

        /// <summary>
        /// Convention for enum names
        /// </summary>
        public NamingStyle EnumName { get; set; } = NamingStyle.PascalCase;

        /// <summary>
        /// Convention for enum member names
        /// </summary>
        public NamingStyle EnumMemberName { get; set; } = NamingStyle.PascalCase;

        /// <summary>
        /// Convention for namespace names
        /// </summary>
        public NamingStyle NamespaceName { get; set; } = NamingStyle.PascalCase;

        /// <summary>
        /// Convention for file names
        /// </summary>
        public NamingStyle FileName { get; set; } = NamingStyle.PascalCase;

        /// <summary>
        /// File extension (e.g., ".cs", ".java")
        /// </summary>
        public string FileExtension { get; set; } = string.Empty;

        /// <summary>
        /// Convert a name to the specified naming style
        /// </summary>
        public static string Convert(string input, NamingStyle style)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return style switch
            {
                NamingStyle.PascalCase => ToPascalCase(input),
                NamingStyle.CamelCase => ToCamelCase(input),
                NamingStyle.SnakeCase => ToSnakeCase(input),
                NamingStyle.UpperSnakeCase => ToSnakeCase(input).ToUpperInvariant(),
                NamingStyle.KebabCase => ToKebabCase(input),
                NamingStyle.LowerCase => input.ToLowerInvariant(),
                NamingStyle.UpperCase => input.ToUpperInvariant(),
                _ => input
            };
        }

        private static string ToPascalCase(string input)
        {
            var words = SplitIntoWords(input);
            return string.Concat(words.Select(w => 
                char.ToUpperInvariant(w[0]) + w.Substring(1).ToLowerInvariant()));
        }

        private static string ToCamelCase(string input)
        {
            var pascal = ToPascalCase(input);
            if (string.IsNullOrEmpty(pascal)) return pascal;
            return char.ToLowerInvariant(pascal[0]) + pascal.Substring(1);
        }

        private static string ToSnakeCase(string input)
        {
            var words = SplitIntoWords(input);
            return string.Join("_", words.Select(w => w.ToLowerInvariant()));
        }

        private static string ToKebabCase(string input)
        {
            var words = SplitIntoWords(input);
            return string.Join("-", words.Select(w => w.ToLowerInvariant()));
        }

        private static string[] SplitIntoWords(string input)
        {
            if (string.IsNullOrEmpty(input)) return Array.Empty<string>();

            // First, split by common separators
            var parts = input.Split(new[] { '_', '-', ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);

            var words = new List<string>();
            foreach (var part in parts)
            {
                // Split by case changes (for PascalCase/camelCase)
                var currentWord = new System.Text.StringBuilder();
                for (int i = 0; i < part.Length; i++)
                {
                    var c = part[i];
                    if (i > 0 && char.IsUpper(c) && !char.IsUpper(part[i - 1]))
                    {
                        if (currentWord.Length > 0)
                        {
                            words.Add(currentWord.ToString());
                            currentWord.Clear();
                        }
                    }
                    currentWord.Append(c);
                }
                if (currentWord.Length > 0)
                {
                    words.Add(currentWord.ToString());
                }
            }

            return words.ToArray();
        }
    }
}
