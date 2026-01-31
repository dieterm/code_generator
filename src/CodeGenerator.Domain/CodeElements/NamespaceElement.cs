namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a namespace declaration that contains types
    /// </summary>
    public class NamespaceElement : CodeElement
    {
        /// <summary>
        /// Full namespace path (e.g., "MyCompany.MyProduct.Domain")
        /// </summary>
        public string FullName
        {
            get => Name ?? string.Empty;
            set => Name = value;
        }

        /// <summary>
        /// Using/import statements within this namespace
        /// </summary>
        public List<UsingElement> Usings { get; set; } = new();

        /// <summary>
        /// Types declared within this namespace
        /// </summary>
        public List<TypeElement> Types { get; set; } = new();

        /// <summary>
        /// Nested namespaces
        /// </summary>
        public List<NamespaceElement> NestedNamespaces { get; set; } = new();

        /// <summary>
        /// Use file-scoped namespace syntax (C# 10+)
        /// </summary>
        public bool IsFileScoped { get; set; } = true;

        public NamespaceElement() { }

        public NamespaceElement(string fullName)
        {
            FullName = fullName;
        }
    }
}
