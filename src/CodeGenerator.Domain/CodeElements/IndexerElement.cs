namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents an indexer declaration
    /// </summary>
    public class IndexerElement : CodeElement
    {
        /// <summary>
        /// Return type of the indexer
        /// </summary>
        public TypeReference Type { get; set; } = new();

        /// <summary>
        /// Parameters of the indexer (the index values)
        /// </summary>
        public List<ParameterElement> Parameters { get; set; } = new();

        /// <summary>
        /// Whether this indexer has a getter
        /// </summary>
        public bool HasGetter { get; set; } = true;

        /// <summary>
        /// Whether this indexer has a setter
        /// </summary>
        public bool HasSetter { get; set; } = true;

        /// <summary>
        /// Getter body
        /// </summary>
        public CompositeStatement GetterBody { get; set; } = new();

        /// <summary>
        /// Setter body
        /// </summary>
        public CompositeStatement SetterBody { get; set; } = new();

        /// <summary>
        /// Access modifier for the getter (if different from indexer)
        /// </summary>
        public AccessModifier? GetterAccessModifier { get; set; }

        /// <summary>
        /// Access modifier for the setter (if different from indexer)
        /// </summary>
        public AccessModifier? SetterAccessModifier { get; set; }

        public IndexerElement() { }

        public IndexerElement(TypeReference type, params ParameterElement[] parameters)
        {
            Type = type;
            Parameters = parameters.ToList();
        }
    }
}
