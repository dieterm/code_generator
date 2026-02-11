namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a constructor declaration
    /// </summary>
    public class ConstructorElement : CodeElement
    {
        /// <summary>
        /// Parameters of the constructor
        /// </summary>
        public List<ParameterElement> Parameters { get; set; } = new();

        /// <summary>
        /// Constructor body (statements as code string)
        /// </summary>
        public CompositeStatement Body { get; set; } = new();

        /// <summary>
        /// Base constructor call (null if not calling base)
        /// </summary>
        public ConstructorInitializer? BaseCall { get; set; }

        /// <summary>
        /// This constructor call for constructor chaining (null if not chaining)
        /// </summary>
        public ConstructorInitializer? ThisCall { get; set; }

        /// <summary>
        /// Whether this is a primary constructor (C# 12)
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Whether this is a static constructor
        /// </summary>
        public bool IsStatic { get; set; }

        public ConstructorElement() { }

        public ConstructorElement(params ParameterElement[] parameters)
        {
            Parameters = parameters.ToList();
        }

        /// <summary>
        /// Add a parameter to this constructor
        /// </summary>
        public ConstructorElement AddParameter(string name, TypeReference type)
        {
            Parameters.Add(new ParameterElement(name, type));
            return this;
        }

        /// <summary>
        /// Set the base constructor call
        /// </summary>
        public ConstructorElement WithBaseCall(params string[] arguments)
        {
            BaseCall = new ConstructorInitializer { Arguments = arguments.ToList() };
            return this;
        }

        /// <summary>
        /// Set the this constructor call
        /// </summary>
        public ConstructorElement WithThisCall(params string[] arguments)
        {
            ThisCall = new ConstructorInitializer { Arguments = arguments.ToList() };
            return this;
        }
    }

    /// <summary>
    /// Represents a constructor initializer (base or this call)
    /// </summary>
    public class ConstructorInitializer
    {
        /// <summary>
        /// Arguments passed to the base/this constructor (as code strings)
        /// </summary>
        public List<string> Arguments { get; set; } = new();
    }

    /// <summary>
    /// Represents a finalizer/destructor
    /// </summary>
    public class FinalizerElement : CodeElement
    {
        /// <summary>
        /// Finalizer body (statements as code string)
        /// </summary>
        public string? Body { get; set; }
    }
}
