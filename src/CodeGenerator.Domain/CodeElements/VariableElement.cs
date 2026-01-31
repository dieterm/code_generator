namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a local variable declaration
    /// </summary>
    public class VariableElement : CodeElement
    {
        /// <summary>
        /// Type of the variable (null for var/auto inference)
        /// </summary>
        public TypeReference? Type { get; set; }

        /// <summary>
        /// Whether to use type inference (var in C#, auto in C++)
        /// </summary>
        public bool UseTypeInference { get; set; }

        /// <summary>
        /// Initial value expression (as code string)
        /// </summary>
        public string? InitialValue { get; set; }

        /// <summary>
        /// Whether this variable has an initial value
        /// </summary>
        public bool HasInitialValue => !string.IsNullOrEmpty(InitialValue);

        /// <summary>
        /// Whether this is a constant (const in C#, final in Java)
        /// </summary>
        public bool IsConstant { get; set; }

        /// <summary>
        /// Whether this is using declaration (C# 8 using var)
        /// </summary>
        public bool IsUsing { get; set; }

        public VariableElement() { }

        public VariableElement(string name, TypeReference type)
        {
            Name = name;
            Type = type;
        }

        public VariableElement(string name, TypeReference type, string initialValue)
        {
            Name = name;
            Type = type;
            InitialValue = initialValue;
        }

        /// <summary>
        /// Create a var-inferred variable
        /// </summary>
        public static VariableElement Var(string name, string initialValue)
        {
            return new VariableElement
            {
                Name = name,
                UseTypeInference = true,
                InitialValue = initialValue
            };
        }

        /// <summary>
        /// Create a constant variable
        /// </summary>
        public static VariableElement Const(string name, TypeReference type, string value)
        {
            return new VariableElement(name, type, value)
            {
                IsConstant = true
            };
        }
    }
}
