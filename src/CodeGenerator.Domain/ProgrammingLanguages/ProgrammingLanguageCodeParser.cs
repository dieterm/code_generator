using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Domain.ProgrammingLanguages
{
    /// <summary>
    /// Base class for programming language code parsers.
    /// Parses source code strings into CodeElement structures for a specific programming language.
    /// </summary>
    public abstract class ProgrammingLanguageCodeParser
    {
        /// <summary>
        /// The programming language this parser handles
        /// </summary>
        public abstract ProgrammingLanguage Language { get; }

        /// <summary>
        /// Parse a complete source code file into a CodeFileElement
        /// </summary>
        /// <param name="sourceCode">The source code to parse</param>
        /// <param name="fileName">The file name (without extension)</param>
        /// <returns>A CodeFileElement representing the parsed source code</returns>
        public abstract CodeFileElement ParseCodeFile(string sourceCode, string fileName);
    }
}
