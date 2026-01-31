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

    /// <summary>
    /// Represents a reference to a type (for use in parameters, return types, base types, etc.)
    /// </summary>
    public class TypeReference
    {
        /// <summary>
        /// Full type name (e.g., "System.Collections.Generic.List")
        /// </summary>
        public string TypeName { get; set; } = string.Empty;

        /// <summary>
        /// Generic type arguments if this is a generic type
        /// </summary>
        public List<TypeReference> GenericArguments { get; set; } = new();

        /// <summary>
        /// Whether this type is nullable
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Whether this is an array type
        /// </summary>
        public bool IsArray { get; set; }

        /// <summary>
        /// Array rank (dimensions) if IsArray is true
        /// </summary>
        public int ArrayRank { get; set; } = 1;

        public TypeReference() { }

        public TypeReference(string typeName)
        {
            TypeName = typeName;
        }

        public TypeReference(string typeName, bool isNullable)
        {
            TypeName = typeName;
            IsNullable = isNullable;
        }

        /// <summary>
        /// Creates a generic type reference
        /// </summary>
        public static TypeReference Generic(string typeName, params TypeReference[] typeArguments)
        {
            return new TypeReference(typeName)
            {
                GenericArguments = typeArguments.ToList()
            };
        }

        /// <summary>
        /// Common type references
        /// </summary>
        public static class Common
        {
            public static TypeReference Void => new("void");
            public static TypeReference String => new("string");
            public static TypeReference Int => new("int");
            public static TypeReference Long => new("long");
            public static TypeReference Bool => new("bool");
            public static TypeReference Double => new("double");
            public static TypeReference Decimal => new("decimal");
            public static TypeReference Object => new("object");
            public static TypeReference DateTime => new("DateTime");
            public static TypeReference Guid => new("Guid");
            public static TypeReference Task => new("Task");
            public static TypeReference TaskOf(TypeReference innerType) => Generic("Task", innerType);
            public static TypeReference ListOf(TypeReference innerType) => Generic("List", innerType);
            public static TypeReference IEnumerableOf(TypeReference innerType) => Generic("IEnumerable", innerType);
            public static TypeReference DictionaryOf(TypeReference keyType, TypeReference valueType) => Generic("Dictionary", keyType, valueType);
        }
    }
}
