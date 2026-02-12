namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents an if statement
    /// </summary>
    public class IfStatementElement : StatementElement
    {
        /// <summary>
        /// The condition expression
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Statements to execute if condition is true
        /// </summary>
        public CompositeStatement ThenStatements { get; set; } = new();

        /// <summary>
        /// Statements to execute if condition is false
        /// </summary>
        public CompositeStatement ElseStatements { get; set; } = new();

        /// <summary>
        /// Else-if branches
        /// </summary>
        public List<ElseIfBranch> ElseIfBranches { get; set; } = new();

        public IfStatementElement() { }

        public IfStatementElement(string condition)
        {
            Condition = condition;
        }
    }
}
