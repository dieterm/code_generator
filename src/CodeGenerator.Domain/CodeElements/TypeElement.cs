namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Base class for type declarations (class, interface, struct, enum, record)
    /// </summary>
    public abstract class TypeElement : CodeElement
    {
        /// <summary>
        /// Generic type parameters
        /// </summary>
        public List<GenericTypeParameterElement> GenericTypeParameters { get; set; } = new();

        /// <summary>
        /// Base types this type inherits from or implements
        /// </summary>
        public List<TypeReference> BaseTypes { get; set; } = new();

        /// <summary>
        /// Type constraints for generic parameters
        /// </summary>
        public List<GenericConstraintElement> GenericConstraints { get; set; } = new();

        /// <summary>
        /// Nested types within this type
        /// </summary>
        public List<TypeElement> NestedTypes { get; set; } = new();

        /// <summary>
        /// Whether this type is a generic type
        /// </summary>
        public bool IsGeneric => GenericTypeParameters.Count > 0;
    }
}
