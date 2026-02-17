namespace CodeGenerator.Domain.CodeElements
{
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
