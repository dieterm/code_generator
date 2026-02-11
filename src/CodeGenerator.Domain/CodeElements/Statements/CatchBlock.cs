namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a catch block
    /// </summary>
    public class CatchBlock : StatementElement
    {
        /// <summary>
        /// Exception type to catch (null for catch-all)
        /// </summary>
        public TypeReference? ExceptionType { get; set; }

        /// <summary>
        /// Exception variable name
        /// </summary>
        public string? ExceptionVariable { get; set; }

        /// <summary>
        /// When filter expression (C# 6+)
        /// </summary>
        public string? WhenFilter { get; set; }

        /// <summary>
        /// Catch block statements
        /// </summary>
        public List<StatementElement> Statements { get; set; } = new();
    }
}
