namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a using/import statement
    /// </summary>
    public class UsingElement : CodeElement
    {
        /// <summary>
        /// The namespace or module being imported
        /// </summary>
        public string Namespace
        {
            get => Name ?? string.Empty;
            set => Name = value;
        }

        /// <summary>
        /// Alias for the using (e.g., "using Alias = Some.Long.Namespace")
        /// </summary>
        public string? Alias { get; set; }

        /// <summary>
        /// Whether this is a static using (C#)
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Whether this is a global using (C# 10+)
        /// </summary>
        public bool IsGlobal { get; set; }

        public UsingElement() { }

        public UsingElement(string @namespace)
        {
            Namespace = @namespace;
        }

        public UsingElement(string @namespace, string alias)
        {
            Namespace = @namespace;
            Alias = alias;
        }
    }
}
