namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a return statement
    /// </summary>
    public class ReturnStatementElement : StatementElement
    {
        /// <summary>
        /// The expression to return (null for void return)
        /// </summary>
        public string? Expression { get; set; }

        public ReturnStatementElement() { }

        public ReturnStatementElement(string expression)
        {
            Expression = expression;
        }
    }
}
