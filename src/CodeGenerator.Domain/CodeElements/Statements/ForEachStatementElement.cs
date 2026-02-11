namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a foreach loop
    /// </summary>
    public class ForEachStatementElement : StatementElement
    {
        /// <summary>
        /// Type of the iteration variable (null for var)
        /// </summary>
        public TypeReference? VariableType { get; set; }

        /// <summary>
        /// Name of the iteration variable
        /// </summary>
        public string VariableName { get; set; } = string.Empty;

        /// <summary>
        /// The collection to iterate over
        /// </summary>
        public string Collection { get; set; } = string.Empty;

        /// <summary>
        /// Loop body statements
        /// </summary>
        public List<StatementElement> Body { get; set; } = new();

        public ForEachStatementElement() { }

        public ForEachStatementElement(string variableName, string collection)
        {
            VariableName = variableName;
            Collection = collection;
        }
    }
}
