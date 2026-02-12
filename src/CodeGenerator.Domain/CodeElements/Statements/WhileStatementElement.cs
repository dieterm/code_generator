namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a while loop
    /// </summary>
    public class WhileStatementElement : StatementElement
    {
        /// <summary>
        /// Loop condition
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Loop body statements
        /// </summary>
        public CompositeStatement Body { get; set; } = new();

        public WhileStatementElement() { }

        public WhileStatementElement(string condition)
        {
            Condition = condition;
        }
    }
}
