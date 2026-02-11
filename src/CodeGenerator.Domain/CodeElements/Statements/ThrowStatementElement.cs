namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a throw statement
    /// </summary>
    public class ThrowStatementElement : StatementElement
    {
        /// <summary>
        /// The exception expression to throw
        /// </summary>
        public string? Expression { get; set; }

        public ThrowStatementElement() { }

        public ThrowStatementElement(string expression)
        {
            Expression = expression;
        }
    }
}
