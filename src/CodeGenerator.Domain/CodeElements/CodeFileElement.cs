namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Represents a complete source code file
    /// </summary>
    public class CodeFileElement : CodeElement
    {
        /// <summary>
        /// File name (without extension)
        /// </summary>
        public string FileName
        {
            get => Name ?? string.Empty;
            set => Name = value;
        }

        /// <summary>
        /// File extension (e.g., ".cs", ".vb", ".py", ".js")
        /// </summary>
        public string FileExtension { get; set; } = ".cs";

        /// <summary>
        /// Full file name with extension
        /// </summary>
        public string FullFileName => $"{FileName}{FileExtension}";

        /// <summary>
        /// File-level using/import statements
        /// </summary>
        public List<UsingElement> Usings { get; set; } = new();

        /// <summary>
        /// File-level attributes (e.g., assembly attributes)
        /// </summary>
        public List<AttributeElement> FileAttributes { get; set; } = new();

        /// <summary>
        /// Namespaces in this file
        /// </summary>
        public List<NamespaceElement> Namespaces { get; set; } = new();

        /// <summary>
        /// Top-level types (for languages that support types outside namespaces)
        /// </summary>
        public List<TypeElement> TopLevelTypes { get; set; } = new();

        /// <summary>
        /// Top-level statements (C# 9+ top-level programs)
        /// </summary>
        public List<string> TopLevelStatements { get; set; } = new();

        /// <summary>
        /// File header comment/documentation
        /// </summary>
        public string? FileHeader { get; set; }

        /// <summary>
        /// Whether to use nullable reference types
        /// </summary>
        public bool? NullableContext { get; set; }

        /// <summary>
        /// Whether to enable implicit usings
        /// </summary>
        public bool UseImplicitUsings { get; set; }

        /// <summary>
        /// Global using statements (C# 10+)
        /// </summary>
        public List<UsingElement> GlobalUsings { get; set; } = new();

        /// <summary>
        /// The target programming language
        /// </summary>
        public ProgrammingLanguage Language { get; set; } = ProgrammingLanguage.CSharp;

        public CodeFileElement() { }

        public CodeFileElement(string fileName)
        {
            FileName = fileName;
        }

        public CodeFileElement(string fileName, ProgrammingLanguage language)
        {
            FileName = fileName;
            Language = language;
            FileExtension = GetDefaultExtension(language);
        }

        /// <summary>
        /// Add a using/import statement
        /// </summary>
        public CodeFileElement AddUsing(string @namespace)
        {
            if(!Usings.Any(u => u.Namespace == @namespace))
                Usings.Add(new UsingElement(@namespace));
            return this;
        }

        /// <summary>
        /// Add a namespace with a single type
        /// </summary>
        public CodeFileElement AddNamespace(string namespaceName, TypeElement type)
        {
            var ns = new NamespaceElement(namespaceName);
            ns.Types.Add(type);
            Namespaces.Add(ns);
            return this;
        }

        private static string GetDefaultExtension(ProgrammingLanguage language)
        {
            return language switch
            {
                ProgrammingLanguage.CSharp => ".cs",
                ProgrammingLanguage.VisualBasic => ".vb",
                ProgrammingLanguage.FSharp => ".fs",
                ProgrammingLanguage.Python => ".py",
                ProgrammingLanguage.JavaScript => ".js",
                ProgrammingLanguage.TypeScript => ".ts",
                ProgrammingLanguage.Java => ".java",
                ProgrammingLanguage.Kotlin => ".kt",
                ProgrammingLanguage.Swift => ".swift",
                ProgrammingLanguage.Go => ".go",
                ProgrammingLanguage.Rust => ".rs",
                ProgrammingLanguage.Cpp => ".cpp",
                ProgrammingLanguage.CppHeader => ".h",
                ProgrammingLanguage.Php => ".php",
                ProgrammingLanguage.Ruby => ".rb",
                _ => ".txt"
            };
        }
    }

    /// <summary>
    /// Supported programming languages for code generation
    /// </summary>
    public enum ProgrammingLanguage
    {
        CSharp,
        VisualBasic,
        FSharp,
        Python,
        JavaScript,
        TypeScript,
        Java,
        Kotlin,
        Swift,
        Go,
        Rust,
        Cpp,
        CppHeader,
        Php,
        Ruby,
        Sql,
        Html,
        Css,
        Xml,
        Json,
        Yaml,
        Other
    }
}
