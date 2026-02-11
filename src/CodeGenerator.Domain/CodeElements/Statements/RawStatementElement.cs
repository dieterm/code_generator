namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a raw code statement (as a string)
    /// </summary>
    public class RawStatementElement : StatementElement
    {
        /// <summary>
        /// The raw code statement
        /// </summary>
        public string Code { get; set; } = string.Empty;

        public RawStatementElement() { }

        public RawStatementElement(string code)
        {
            Code = code;
        }

        public static implicit operator RawStatementElement(string code) => new(code);
    }
}
