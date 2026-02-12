namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a for loop
    /// </summary>
    public class ForStatementElement : StatementElement
    {
        /// <summary>
        /// Loop initializer
        /// </summary>
        public string? Initializer { get; set; }

        /// <summary>
        /// Loop condition
        /// </summary>
        public string? Condition { get; set; }

        /// <summary>
        /// Loop incrementer
        /// </summary>
        public string? Incrementer { get; set; }

        /// <summary>
        /// Loop body statements
        /// </summary>
        public CompositeStatement Body { get; set; } = new();
    }
}
