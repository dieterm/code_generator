namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a try-catch-finally statement
    /// </summary>
    public class TryCatchStatementElement : StatementElement
    {
        /// <summary>
        /// Try block statements
        /// </summary>
        public CompositeStatement TryStatements { get; set; } = new();

        /// <summary>
        /// Catch blocks
        /// </summary>
        public List<CatchBlock> CatchBlocks { get; set; } = new();

        /// <summary>
        /// Finally block statements
        /// </summary>
        public CompositeStatement FinallyStatements { get; set; } = new();

        /// <summary>
        /// Whether there is a finally block
        /// </summary>
        public bool HasFinally => FinallyStatements.Statements.Count > 0;
    }
}
