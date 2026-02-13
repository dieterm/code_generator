namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents an operator declaration
    /// </summary>
    public class OperatorElement : CodeElement
    {
        /// <summary>
        /// The operator symbol or keyword
        /// </summary>
        public OperatorType OperatorType { get; set; }

        /// <summary>
        /// Return type of the operator
        /// </summary>
        public TypeReference ReturnType { get; set; } = new();

        /// <summary>
        /// Parameters of the operator
        /// </summary>
        public List<ParameterElement> Parameters { get; set; } = new();

        /// <summary>
        /// Operator body
        /// </summary>
        //public string? Body { get; set; }
        /// <summary>
        /// Statements in the method body
        /// </summary>
        public CompositeStatement Body { get; set; } = new();

        /// <summary>
        /// Whether this is an implicit or explicit conversion operator
        /// </summary>
        public bool IsImplicit { get; set; }

        public OperatorElement() { }

        public OperatorElement(OperatorType operatorType, TypeReference returnType)
        {
            OperatorType = operatorType;
            ReturnType = returnType;
        }
    }

    /// <summary>
    /// Types of operators that can be overloaded
    /// </summary>
    public enum OperatorType
    {
        // Unary operators
        UnaryPlus,
        UnaryMinus,
        LogicalNot,
        BitwiseNot,
        Increment,
        Decrement,
        True,
        False,

        // Binary operators
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Modulus,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
        LeftShift,
        RightShift,
        UnsignedRightShift,

        // Comparison operators
        Equality,
        Inequality,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,

        // Conversion operators
        Implicit,
        Explicit
    }
}
