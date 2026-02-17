namespace CodeGenerator.Domain.CodeElements
{
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
}
