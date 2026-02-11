namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents an attribute/annotation applied to a code element
    /// </summary>
    public class AttributeElement : CodeElement
    {
        /// <summary>
        /// Name of the attribute (without Attribute suffix)
        /// </summary>
        public string AttributeName
        {
            get => Name ?? string.Empty;
            set => Name = value;
        }

        /// <summary>
        /// Positional arguments for the attribute
        /// </summary>
        public List<string> Arguments { get; set; } = new();

        /// <summary>
        /// Named arguments for the attribute
        /// </summary>
        public Dictionary<string, string> NamedArguments { get; set; } = new();

        /// <summary>
        /// Target of the attribute (for assembly, module, return, etc.)
        /// </summary>
        public AttributeTarget Target { get; set; } = AttributeTarget.Default;

        public AttributeElement() { }

        public AttributeElement(string attributeName)
        {
            AttributeName = attributeName;
        }

        public AttributeElement(string attributeName, params string[] arguments)
        {
            AttributeName = attributeName;
            Arguments = arguments.ToList();
        }

        /// <summary>
        /// Add a positional argument
        /// </summary>
        public AttributeElement AddArgument(string value)
        {
            Arguments.Add(value);
            return this;
        }

        /// <summary>
        /// Add a named argument
        /// </summary>
        public AttributeElement AddNamedArgument(string name, string value)
        {
            NamedArguments[name] = value;
            return this;
        }

        /// <summary>
        /// Common attributes
        /// </summary>
        public static class Common
        {
            public static AttributeElement Obsolete(string? message = null, bool isError = false)
            {
                var attr = new AttributeElement("Obsolete");
                if (message != null)
                {
                    attr.Arguments.Add($"\"{message}\"");
                    if (isError)
                        attr.Arguments.Add("true");
                }
                return attr;
            }

            public static AttributeElement Serializable => new("Serializable");
            
            public static AttributeElement Flags => new("Flags");
            
            public static AttributeElement Required => new("Required");

            public static AttributeElement JsonPropertyName(string name) => 
                new AttributeElement("JsonPropertyName", $"\"{name}\"");

            public static AttributeElement XmlElement(string name) =>
                new AttributeElement("XmlElement", $"\"{name}\"");

            public static AttributeElement DataMember(string? name = null, int order = -1)
            {
                var attr = new AttributeElement("DataMember");
                if (name != null)
                    attr.NamedArguments["Name"] = $"\"{name}\"";
                if (order >= 0)
                    attr.NamedArguments["Order"] = order.ToString();
                return attr;
            }
        }
    }
}
