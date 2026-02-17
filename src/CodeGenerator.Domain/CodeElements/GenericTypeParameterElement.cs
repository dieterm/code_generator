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
}
