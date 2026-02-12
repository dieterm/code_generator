namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a using statement (resource disposal)
    /// </summary>
    public class UsingStatementElement : StatementElement
    {
        /// <summary>
        /// Resource variable declaration or expression
        /// </summary>
        public string Resource { get; set; } = string.Empty;

        /// <summary>
        /// Using block statements
        /// </summary>
        public CompositeStatement Body { get; set; } = new();

        /// <summary>
        /// Whether this is a using declaration (C# 8+)
        /// </summary>
        public bool IsDeclaration { get; set; }
    }
}
