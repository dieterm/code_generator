namespace CodeGenerator.Domain.CodeElements
{
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
}
