namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a generic type parameter
    /// </summary>
    public class GenericTypeParameterElement : CodeElement
    {
        /// <summary>
        /// Variance modifier for this type parameter
        /// </summary>
        public GenericVariance Variance { get; set; } = GenericVariance.Invariant;

        public GenericTypeParameterElement() { }

        public GenericTypeParameterElement(string name)
        {
            Name = name;
        }

        public GenericTypeParameterElement(string name, GenericVariance variance)
        {
            Name = name;
            Variance = variance;
        }
    }

    /// <summary>
    /// Variance modifiers for generic type parameters
    /// </summary>
    public enum GenericVariance
    {
        Invariant,
        Covariant,      // out
        Contravariant   // in
    }

    /// <summary>
    /// Represents a generic constraint
    /// </summary>
    public class GenericConstraintElement : CodeElement
    {
        /// <summary>
        /// Name of the type parameter this constraint applies to
        /// </summary>
        public string TypeParameterName { get; set; } = string.Empty;

        /// <summary>
        /// Constraint types (where T : IInterface, BaseClass)
        /// </summary>
        public List<TypeReference> ConstraintTypes { get; set; } = new();

        /// <summary>
        /// Special constraints
        /// </summary>
        public GenericConstraintKind ConstraintKind { get; set; } = GenericConstraintKind.None;

        public GenericConstraintElement() { }

        public GenericConstraintElement(string typeParameterName)
        {
            TypeParameterName = typeParameterName;
        }

        /// <summary>
        /// Add a type constraint
        /// </summary>
        public GenericConstraintElement AddTypeConstraint(TypeReference type)
        {
            ConstraintTypes.Add(type);
            return this;
        }

        /// <summary>
        /// Add a class constraint
        /// </summary>
        public GenericConstraintElement WithClassConstraint()
        {
            ConstraintKind |= GenericConstraintKind.Class;
            return this;
        }

        /// <summary>
        /// Add a struct constraint
        /// </summary>
        public GenericConstraintElement WithStructConstraint()
        {
            ConstraintKind |= GenericConstraintKind.Struct;
            return this;
        }

        /// <summary>
        /// Add a new() constraint
        /// </summary>
        public GenericConstraintElement WithNewConstraint()
        {
            ConstraintKind |= GenericConstraintKind.New;
            return this;
        }

        /// <summary>
        /// Add a notnull constraint
        /// </summary>
        public GenericConstraintElement WithNotNullConstraint()
        {
            ConstraintKind |= GenericConstraintKind.NotNull;
            return this;
        }

        /// <summary>
        /// Add an unmanaged constraint
        /// </summary>
        public GenericConstraintElement WithUnmanagedConstraint()
        {
            ConstraintKind |= GenericConstraintKind.Unmanaged;
            return this;
        }
    }

    /// <summary>
    /// Special generic constraint kinds
    /// </summary>
    [Flags]
    public enum GenericConstraintKind
    {
        None = 0,
        Class = 1 << 0,         // where T : class
        Struct = 1 << 1,        // where T : struct
        New = 1 << 2,           // where T : new()
        NotNull = 1 << 3,       // where T : notnull
        Unmanaged = 1 << 4,     // where T : unmanaged
        Default = 1 << 5        // where T : default (C# 9)
    }
}
