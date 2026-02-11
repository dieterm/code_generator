namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a switch statement
    /// </summary>
    public class SwitchStatementElement : StatementElement
    {
        /// <summary>
        /// Expression being switched on
        /// </summary>
        public string Expression { get; set; } = string.Empty;

        /// <summary>
        /// Switch cases
        /// </summary>
        public List<SwitchCase> Cases { get; set; } = new();

        /// <summary>
        /// Default case statements
        /// </summary>
        public List<StatementElement> DefaultStatements { get; set; } = new();
    }
}
