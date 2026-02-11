using CodeGenerator.Domain.ProgrammingLanguages;

namespace CodeGenerator.Domain.ProgrammingLanguages.Python
{
    /// <summary>
    /// Extension methods for registering the Python code generator
    /// </summary>
    public static class PythonCodeGeneratorExtensions
    {
        /// <summary>
        /// Register the Python code generator with the global registry
        /// </summary>
        public static void RegisterPythonCodeGenerator()
        {
            ProgrammingLanguageCodeGenerators.Register(
                PythonLanguage.Instance.Id,
                () => new PythonCodeGenerator());
        }

        /// <summary>
        /// Get a new instance of the Python code generator
        /// </summary>
        public static PythonCodeGenerator CreatePythonCodeGenerator()
        {
            return new PythonCodeGenerator();
        }
    }
}
