namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a switch case
    /// </summary>
    public class SwitchCase : StatementElement
    {
        /// <summary>
        /// Case label values
        /// </summary>
        public List<string> Labels { get; set; } = new();

        /// <summary>
        /// Case pattern (for pattern matching)
        /// </summary>
        public string? Pattern { get; set; }

        /// <summary>
        /// When clause (for pattern matching)
        /// </summary>
        public string? WhenClause { get; set; }

        /// <summary>
        /// Case statements
        /// </summary>
        public List<StatementElement> Statements { get; set; } = new();
    }
}
