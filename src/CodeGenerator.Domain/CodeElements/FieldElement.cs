namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a field declaration
    /// </summary>
    public class FieldElement : CodeElement
    {
        /// <summary>
        /// Type of the field
        /// </summary>
        public TypeReference Type { get; set; } = new();

        /// <summary>
        /// Initial value expression (as code string)
        /// </summary>
        public string? InitialValue { get; set; }

        /// <summary>
        /// Whether this field has an initial value
        /// </summary>
        public bool HasInitialValue => !string.IsNullOrEmpty(InitialValue);

        public FieldElement()
        {
            AccessModifier = AccessModifier.Private;
        }

        public FieldElement(string name, TypeReference type)
        {
            Name = name;
            Type = type;
            AccessModifier = AccessModifier.Private;
        }

        public FieldElement(string name, TypeReference type, string? initialValue)
        {
            Name = name;
            Type = type;
            InitialValue = initialValue;
            AccessModifier = AccessModifier.Private;
        }

        /// <summary>
        /// Create a backing field for a property
        /// </summary>
        public static FieldElement BackingField(string propertyName, TypeReference type)
        {
            return new FieldElement($"_{char.ToLower(propertyName[0])}{propertyName.Substring(1)}", type)
            {
                AccessModifier = AccessModifier.Private
            };
        }
    }
}
