namespace CodeGenerator.Domain.CodeElements
{
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
