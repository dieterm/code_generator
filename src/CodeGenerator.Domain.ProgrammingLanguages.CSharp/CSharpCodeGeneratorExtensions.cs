using CodeGenerator.Domain.ProgrammingLanguages;

namespace CodeGenerator.Domain.ProgrammingLanguages.CSharp
{
    /// <summary>
    /// Extension methods for registering the C# code generator
    /// </summary>
    public static class CSharpCodeGeneratorExtensions
    {
        /// <summary>
        /// Register the C# code generator with the global registry
        /// </summary>
        public static void RegisterCSharpCodeGenerator()
        {
            ProgrammingLanguageCodeGenerators.Register(
                CSharpLanguage.Instance.Id, 
                () => new CSharpCodeGenerator());
        }

        /// <summary>
        /// Get a new instance of the C# code generator
        /// </summary>
        public static CSharpCodeGenerator CreateCSharpCodeGenerator()
        {
            return new CSharpCodeGenerator();
        }
    }
}
