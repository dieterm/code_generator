namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a method/constructor parameter
    /// </summary>
    public class ParameterElement : CodeElement
    {
        /// <summary>
        /// Type of the parameter
        /// </summary>
        public TypeReference Type { get; set; } = new();

        /// <summary>
        /// Parameter modifier (ref, out, in, params)
        /// </summary>
        public ParameterModifier Modifier { get; set; } = ParameterModifier.None;

        /// <summary>
        /// Default value expression (as code string) for optional parameters
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Whether this parameter has a default value
        /// </summary>
        public bool HasDefaultValue => !string.IsNullOrEmpty(DefaultValue);

        /// <summary>
        /// Whether this is an optional parameter
        /// </summary>
        public bool IsOptional => HasDefaultValue;

        /// <summary>
        /// Whether this is the 'this' parameter for extension methods
        /// </summary>
        public bool IsExtensionMethodThis { get; set; }

        public ParameterElement() { }

        public ParameterElement(string name, TypeReference type)
        {
            Name = name;
            Type = type;
        }

        public ParameterElement(string name, TypeReference type, ParameterModifier modifier)
        {
            Name = name;
            Type = type;
            Modifier = modifier;
        }

        public ParameterElement(string name, TypeReference type, string defaultValue)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
        }
    }

    /// <summary>
    /// Parameter modifiers
    /// </summary>
    public enum ParameterModifier
    {
        None,
        Ref,
        Out,
        In,
        Params,
        /// <summary>For Python *args</summary>
        VarArgs,
        /// <summary>For Python **kwargs</summary>
        KeywordArgs
    }
}
