namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a struct declaration
    /// </summary>
    public class StructElement : TypeElement
    {
        /// <summary>
        /// Fields declared in this struct
        /// </summary>
        public List<FieldElement> Fields { get; set; } = new();

        /// <summary>
        /// Properties declared in this struct
        /// </summary>
        public List<PropertyElement> Properties { get; set; } = new();

        /// <summary>
        /// Methods declared in this struct
        /// </summary>
        public List<MethodElement> Methods { get; set; } = new();

        /// <summary>
        /// Constructors declared in this struct
        /// </summary>
        public List<ConstructorElement> Constructors { get; set; } = new();

        /// <summary>
        /// Events declared in this struct
        /// </summary>
        public List<EventElement> Events { get; set; } = new();

        /// <summary>
        /// Whether this is a record struct (C# 10+)
        /// </summary>
        public bool IsRecord { get; set; }

        /// <summary>
        /// Whether this is a readonly struct
        /// </summary>
        public bool IsReadonly { get; set; }

        /// <summary>
        /// Whether this is a ref struct
        /// </summary>
        public bool IsRef { get; set; }

        /// <summary>
        /// Primary constructor parameters (for records)
        /// </summary>
        public List<ParameterElement> PrimaryConstructorParameters { get; set; } = new();

        public StructElement() { }

        public StructElement(string name)
        {
            Name = name;
        }
    }
}
