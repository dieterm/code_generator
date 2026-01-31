namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a delegate declaration
    /// </summary>
    public class DelegateElement : TypeElement
    {
        /// <summary>
        /// Return type of the delegate
        /// </summary>
        public TypeReference ReturnType { get; set; } = TypeReference.Common.Void;

        /// <summary>
        /// Parameters of the delegate
        /// </summary>
        public List<ParameterElement> Parameters { get; set; } = new();

        public DelegateElement() { }

        public DelegateElement(string name, TypeReference returnType)
        {
            Name = name;
            ReturnType = returnType;
        }
    }
}
