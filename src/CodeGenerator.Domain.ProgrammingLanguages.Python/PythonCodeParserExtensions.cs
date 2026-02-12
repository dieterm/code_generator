using CodeGenerator.Domain.ProgrammingLanguages;

namespace CodeGenerator.Domain.ProgrammingLanguages.Python
{
    /// <summary>
    /// Extension methods for registering the Python code parser
    /// </summary>
    public static class PythonCodeParserExtensions
    {
        /// <summary>
        /// Register the Python code parser with the global registry
        /// </summary>
        public static void RegisterPythonCodeParser()
        {
            ProgrammingLanguageCodeParsers.Register(
                PythonLanguage.Instance.Id,
                () => new PythonCodeParser());
        }

        /// <summary>
        /// Get a new instance of the Python code parser
        /// </summary>
        public static PythonCodeParser CreatePythonCodeParser()
        {
            return new PythonCodeParser();
        }
    }
}
