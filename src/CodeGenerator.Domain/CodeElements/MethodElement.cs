namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a method declaration
    /// </summary>
    public class MethodElement : CodeElement
    {
        /// <summary>
        /// Return type of the method
        /// </summary>
        public TypeReference ReturnType { get; set; } = TypeReference.Common.Void;

        /// <summary>
        /// Parameters of the method
        /// </summary>
        public List<ParameterElement> Parameters { get; set; } = new();

        /// <summary>
        /// Generic type parameters for this method
        /// </summary>
        public List<GenericTypeParameterElement> GenericTypeParameters { get; set; } = new();

        /// <summary>
        /// Generic constraints for this method
        /// </summary>
        public List<GenericConstraintElement> GenericConstraints { get; set; } = new();

        /// <summary>
        /// Statements in the method body
        /// </summary>
        public CompositeStatement Body { get; set; } = new();

        /// <summary>
        /// Method body (statements as code string)
        /// </summary>
        //public string? Body { get; set; }

        /// <summary>
        /// Whether this method has a body (false for abstract/interface methods)
        /// </summary>
        public bool HasBody => Body.Statements.Count > 0;

        /// <summary>
        /// Whether this is an expression-bodied method
        /// </summary>
        public bool IsExpressionBodied { get; set; }

        /// <summary>
        /// Expression body for expression-bodied methods
        /// </summary>
        public string? ExpressionBody { get; set; }

        /// <summary>
        /// Whether this method is an extension method
        /// </summary>
        public bool IsExtensionMethod { get; set; }

        /// <summary>
        /// Whether this method is generic
        /// </summary>
        public bool IsGeneric => GenericTypeParameters.Count > 0;

        /// <summary>
        /// Local functions within this method
        /// </summary>
        public List<MethodElement> LocalFunctions { get; set; } = new();

        public MethodElement() { }

        public MethodElement(string name)
        {
            Name = name;
        }

        public MethodElement(string name, TypeReference returnType)
        {
            Name = name;
            ReturnType = returnType;
        }

        /// <summary>
        /// Add a parameter to this method
        /// </summary>
        public MethodElement AddParameter(string name, TypeReference type, ParameterModifier modifier = ParameterModifier.None)
        {
            Parameters.Add(new ParameterElement(name, type) { Modifier = modifier });
            return this;
        }

        /// <summary>
        /// Add an optional parameter with default value
        /// </summary>
        public MethodElement AddOptionalParameter(string name, TypeReference type, string defaultValue)
        {
            Parameters.Add(new ParameterElement(name, type) { DefaultValue = defaultValue });
            return this;
        }
    }
}
