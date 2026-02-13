namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Base class for all code elements that can be used to build source code structures.
    /// </summary>
    public abstract class CodeElement
    {
        protected CodeElement()
        {
            AssemblyTypeFullyQualifiedName = GetType().AssemblyQualifiedName ?? string.Empty;
        }
        /// <summary>
        /// Used for json deserialization to determine the concrete type of code element. Should be set to the assembly qualified name of the concrete class.
        /// </summary>
        public string AssemblyTypeFullyQualifiedName { get; set; }

        public string? RawCode { get; set; }

        /// <summary>
        /// Optional name of the code element
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Documentation/comments for this code element
        /// </summary>
        public string? Documentation { get; set; }

        /// <summary>
        /// Additional attributes/annotations applied to this element
        /// </summary>
        public List<AttributeElement> Attributes { get; set; } = new();

        /// <summary>
        /// Access modifier for this element
        /// </summary>
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

        /// <summary>
        /// Additional modifiers (static, abstract, virtual, etc.)
        /// </summary>
        public ElementModifiers Modifiers { get; set; } = ElementModifiers.None;
    }
}
