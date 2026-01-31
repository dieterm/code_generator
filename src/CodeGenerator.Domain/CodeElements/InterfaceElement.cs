namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents an interface declaration
    /// </summary>
    public class InterfaceElement : TypeElement
    {
        /// <summary>
        /// Properties declared in this interface
        /// </summary>
        public List<PropertyElement> Properties { get; set; } = new();

        /// <summary>
        /// Methods declared in this interface
        /// </summary>
        public List<MethodElement> Methods { get; set; } = new();

        /// <summary>
        /// Events declared in this interface
        /// </summary>
        public List<EventElement> Events { get; set; } = new();

        /// <summary>
        /// Indexers declared in this interface
        /// </summary>
        public List<IndexerElement> Indexers { get; set; } = new();

        public InterfaceElement() { }

        public InterfaceElement(string name)
        {
            Name = name;
        }
    }
}
