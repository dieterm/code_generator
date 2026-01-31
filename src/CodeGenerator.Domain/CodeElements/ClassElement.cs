namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a class declaration
    /// </summary>
    public class ClassElement : TypeElement
    {
        /// <summary>
        /// Fields declared in this class
        /// </summary>
        public List<FieldElement> Fields { get; set; } = new();

        /// <summary>
        /// Properties declared in this class
        /// </summary>
        public List<PropertyElement> Properties { get; set; } = new();

        /// <summary>
        /// Methods declared in this class
        /// </summary>
        public List<MethodElement> Methods { get; set; } = new();

        /// <summary>
        /// Constructors declared in this class
        /// </summary>
        public List<ConstructorElement> Constructors { get; set; } = new();

        /// <summary>
        /// Events declared in this class
        /// </summary>
        public List<EventElement> Events { get; set; } = new();

        /// <summary>
        /// Indexers declared in this class
        /// </summary>
        public List<IndexerElement> Indexers { get; set; } = new();

        /// <summary>
        /// Operators declared in this class
        /// </summary>
        public List<OperatorElement> Operators { get; set; } = new();

        /// <summary>
        /// Finalizer/destructor for this class
        /// </summary>
        public FinalizerElement? Finalizer { get; set; }

        /// <summary>
        /// Whether this is a record class (C# 9+)
        /// </summary>
        public bool IsRecord { get; set; }

        /// <summary>
        /// Primary constructor parameters (for records or C# 12 primary constructors)
        /// </summary>
        public List<ParameterElement> PrimaryConstructorParameters { get; set; } = new();

        public ClassElement() { }

        public ClassElement(string name)
        {
            Name = name;
        }
    }
}
