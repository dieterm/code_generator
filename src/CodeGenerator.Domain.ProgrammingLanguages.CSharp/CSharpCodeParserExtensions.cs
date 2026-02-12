using CodeGenerator.Domain.ProgrammingLanguages;

namespace CodeGenerator.Domain.ProgrammingLanguages.CSharp
{
    /// <summary>
    /// Extension methods for registering the C# code parser
    /// </summary>
    public static class CSharpCodeParserExtensions
    {
        /// <summary>
        /// Register the C# code parser with the global registry
        /// </summary>
        public static void RegisterCSharpCodeParser()
        {
            ProgrammingLanguageCodeParsers.Register(
                CSharpLanguage.Instance.Id,
                () => new CSharpCodeParser());
        }

        /// <summary>
        /// Get a new instance of the C# code parser
        /// </summary>
        public static CSharpCodeParser CreateCSharpCodeParser()
        {
            return new CSharpCodeParser();
        }
    }
}
