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
            JavaLanguage.Instance
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
    }
}
