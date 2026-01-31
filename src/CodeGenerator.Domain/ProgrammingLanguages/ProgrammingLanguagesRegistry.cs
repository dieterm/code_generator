using CodeGenerator.Domain.ProgrammingLanguages.Cpp;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;
using CodeGenerator.Domain.ProgrammingLanguages.FSharp;
using CodeGenerator.Domain.ProgrammingLanguages.Go;
using CodeGenerator.Domain.ProgrammingLanguages.Java;
using CodeGenerator.Domain.ProgrammingLanguages.JavaScript;
using CodeGenerator.Domain.ProgrammingLanguages.Kotlin;
using CodeGenerator.Domain.ProgrammingLanguages.Php;
using CodeGenerator.Domain.ProgrammingLanguages.Python;
using CodeGenerator.Domain.ProgrammingLanguages.Ruby;
using CodeGenerator.Domain.ProgrammingLanguages.Rust;
using CodeGenerator.Domain.ProgrammingLanguages.Swift;
using CodeGenerator.Domain.ProgrammingLanguages.TypeScript;
using CodeGenerator.Domain.ProgrammingLanguages.VisualBasic;

namespace CodeGenerator.Domain.ProgrammingLanguages
{
    /// <summary>
    /// Registry for programming language definitions
    /// Provides easy access to language implementations
    /// </summary>
    public static class ProgrammingLanguages
    {
        /// <summary>
        /// Get all available programming languages
        /// </summary>
        public static IEnumerable<ProgrammingLanguage> All => new ProgrammingLanguage[]
        {
            CSharpLanguage.Instance,
            VisualBasicLanguage.Instance,
            FSharpLanguage.Instance,
            JavaLanguage.Instance,
            KotlinLanguage.Instance,
            PythonLanguage.Instance,
            JavaScriptLanguage.Instance,
            TypeScriptLanguage.Instance,
            SwiftLanguage.Instance,
            GoLanguage.Instance,
            RustLanguage.Instance,
            CppLanguage.Instance,
            PhpLanguage.Instance,
            RubyLanguage.Instance
        };

        /// <summary>
        /// Find a language by ID
        /// </summary>
        public static ProgrammingLanguage? FindById(string id)
        {
            return All.FirstOrDefault(lang => lang.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Find a language by name
        /// </summary>
        public static ProgrammingLanguage? FindByName(string name)
        {
            return All.FirstOrDefault(lang => lang.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Find a language by file extension
        /// </summary>
        public static ProgrammingLanguage? FindByFileExtension(string extension)
        {
            // Normalize extension to include dot
            if (!extension.StartsWith("."))
                extension = "." + extension;

            return All.FirstOrDefault(lang => lang.FileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }
    }
}
