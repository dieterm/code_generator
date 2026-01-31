namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a property declaration
    /// </summary>
    public class PropertyElement : CodeElement
    {
        /// <summary>
        /// Type of the property
        /// </summary>
        public TypeReference Type { get; set; } = new();

        /// <summary>
        /// Whether this property has a getter
        /// </summary>
        public bool HasGetter { get; set; } = true;

        /// <summary>
        /// Whether this property has a setter
        /// </summary>
        public bool HasSetter { get; set; } = true;

        /// <summary>
        /// Access modifier for the getter (if different from property)
        /// </summary>
        public AccessModifier? GetterAccessModifier { get; set; }

        /// <summary>
        /// Access modifier for the setter (if different from property)
        /// </summary>
        public AccessModifier? SetterAccessModifier { get; set; }

        /// <summary>
        /// Whether this is an init-only setter (C# 9+)
        /// </summary>
        public bool IsInitOnly { get; set; }

        /// <summary>
        /// Whether this is an auto-implemented property
        /// </summary>
        public bool IsAutoImplemented { get; set; } = true;

        /// <summary>
        /// Getter body expression/statements (for non-auto properties)
        /// </summary>
        public string? GetterBody { get; set; }

        /// <summary>
        /// Setter body expression/statements (for non-auto properties)
        /// </summary>
        public string? SetterBody { get; set; }

        /// <summary>
        /// Initial value expression (as code string)
        /// </summary>
        public string? InitialValue { get; set; }

        /// <summary>
        /// Whether this property has an initial value
        /// </summary>
        public bool HasInitialValue => !string.IsNullOrEmpty(InitialValue);

        /// <summary>
        /// Whether this is an expression-bodied property (get only, single expression)
        /// </summary>
        public bool IsExpressionBodied { get; set; }

        /// <summary>
        /// Expression body for expression-bodied properties
        /// </summary>
        public string? ExpressionBody { get; set; }

        public PropertyElement() { }

        public PropertyElement(string name, TypeReference type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Create a read-only property
        /// </summary>
        public static PropertyElement ReadOnly(string name, TypeReference type)
        {
            return new PropertyElement(name, type)
            {
                HasSetter = false
            };
        }

        /// <summary>
        /// Create a property with private setter
        /// </summary>
        public static PropertyElement WithPrivateSetter(string name, TypeReference type)
        {
            return new PropertyElement(name, type)
            {
                SetterAccessModifier = AccessModifier.Private
            };
        }

        /// <summary>
        /// Create an init-only property
        /// </summary>
        public static PropertyElement InitOnly(string name, TypeReference type)
        {
            return new PropertyElement(name, type)
            {
                IsInitOnly = true
            };
        }
    }
}
