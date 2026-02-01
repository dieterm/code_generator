namespace CodeGenerator.Domain.ProgrammingLanguages
{
    /// <summary>
    /// Registry for programming language code generators.
    /// Provides access to code generators for different programming languages.
    /// </summary>
    public static class ProgrammingLanguageCodeGenerators
    {
        private static readonly Dictionary<string, Func<ProgrammingLanguageCodeGenerator>> _generatorFactories = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Register a code generator factory for a programming language
        /// </summary>
        /// <param name="languageId">The programming language ID</param>
        /// <param name="factory">Factory function to create the generator</param>
        public static void Register(string languageId, Func<ProgrammingLanguageCodeGenerator> factory)
        {
            _generatorFactories[languageId] = factory;
        }

        /// <summary>
        /// Register a code generator factory for a programming language
        /// </summary>
        /// <typeparam name="TGenerator">The generator type</typeparam>
        /// <param name="language">The programming language</param>
        public static void Register<TGenerator>(ProgrammingLanguage language) where TGenerator : ProgrammingLanguageCodeGenerator, new()
        {
            _generatorFactories[language.Id] = () => new TGenerator();
        }

        /// <summary>
        /// Get a code generator for a programming language by ID
        /// </summary>
        /// <param name="languageId">The programming language ID</param>
        /// <returns>A new instance of the code generator, or null if not registered</returns>
        public static ProgrammingLanguageCodeGenerator? GetGenerator(string languageId)
        {
            if (_generatorFactories.TryGetValue(languageId, out var factory))
            {
                return factory();
            }
            return null;
        }

        /// <summary>
        /// Get a code generator for a programming language
        /// </summary>
        /// <param name="language">The programming language</param>
        /// <returns>A new instance of the code generator, or null if not registered</returns>
        public static ProgrammingLanguageCodeGenerator? GetGenerator(ProgrammingLanguage language)
        {
            return GetGenerator(language.Id);
        }

        /// <summary>
        /// Check if a code generator is registered for a programming language
        /// </summary>
        public static bool HasGenerator(string languageId)
        {
            return _generatorFactories.ContainsKey(languageId);
        }

        /// <summary>
        /// Check if a code generator is registered for a programming language
        /// </summary>
        public static bool HasGenerator(ProgrammingLanguage language)
        {
            return HasGenerator(language.Id);
        }

        /// <summary>
        /// Get all registered language IDs
        /// </summary>
        public static IEnumerable<string> RegisteredLanguageIds => _generatorFactories.Keys;

        /// <summary>
        /// Unregister a code generator
        /// </summary>
        public static bool Unregister(string languageId)
        {
            return _generatorFactories.Remove(languageId);
        }

        /// <summary>
        /// Clear all registered generators
        /// </summary>
        public static void Clear()
        {
            _generatorFactories.Clear();
        }
    }
}
