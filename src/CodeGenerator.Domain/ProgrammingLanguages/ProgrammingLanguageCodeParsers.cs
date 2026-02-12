namespace CodeGenerator.Domain.ProgrammingLanguages
{
    /// <summary>
    /// Registry for programming language code parsers.
    /// Provides access to code parsers for different programming languages.
    /// </summary>
    public static class ProgrammingLanguageCodeParsers
    {
        private static readonly Dictionary<string, Func<ProgrammingLanguageCodeParser>> _parserFactories = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Register a code parser factory for a programming language
        /// </summary>
        /// <param name="languageId">The programming language ID</param>
        /// <param name="factory">Factory function to create the parser</param>
        public static void Register(string languageId, Func<ProgrammingLanguageCodeParser> factory)
        {
            _parserFactories[languageId] = factory;
        }

        /// <summary>
        /// Register a code parser factory for a programming language
        /// </summary>
        /// <typeparam name="TParser">The parser type</typeparam>
        /// <param name="language">The programming language</param>
        public static void Register<TParser>(ProgrammingLanguage language) where TParser : ProgrammingLanguageCodeParser, new()
        {
            _parserFactories[language.Id] = () => new TParser();
        }

        /// <summary>
        /// Get a code parser for a programming language by ID
        /// </summary>
        /// <param name="languageId">The programming language ID</param>
        /// <returns>A new instance of the code parser, or null if not registered</returns>
        public static ProgrammingLanguageCodeParser? GetParser(string languageId)
        {
            if (_parserFactories.TryGetValue(languageId, out var factory))
            {
                return factory();
            }
            return null;
        }

        /// <summary>
        /// Get a code parser for a programming language
        /// </summary>
        /// <param name="language">The programming language</param>
        /// <returns>A new instance of the code parser, or null if not registered</returns>
        public static ProgrammingLanguageCodeParser? GetParser(ProgrammingLanguage language)
        {
            return GetParser(language.Id);
        }

        /// <summary>
        /// Check if a code parser is registered for a programming language
        /// </summary>
        public static bool HasParser(string languageId)
        {
            return _parserFactories.ContainsKey(languageId);
        }

        /// <summary>
        /// Check if a code parser is registered for a programming language
        /// </summary>
        public static bool HasParser(ProgrammingLanguage language)
        {
            return HasParser(language.Id);
        }

        /// <summary>
        /// Get all registered language IDs
        /// </summary>
        public static IEnumerable<string> RegisteredLanguageIds => _parserFactories.Keys;

        /// <summary>
        /// Unregister a code parser
        /// </summary>
        public static bool Unregister(string languageId)
        {
            return _parserFactories.Remove(languageId);
        }

        /// <summary>
        /// Clear all registered parsers
        /// </summary>
        public static void Clear()
        {
            _parserFactories.Clear();
        }
    }
}
