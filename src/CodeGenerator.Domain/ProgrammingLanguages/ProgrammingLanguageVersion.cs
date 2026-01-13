namespace CodeGenerator.Domain.ProgrammingLanguages
{
    /// <summary>
    /// Represents a specific version of a programming language
    /// </summary>
    public class ProgrammingLanguageVersion
    {
        /// <summary>
        /// Version identifier (e.g., "12.0", "17", "3.11")
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Display name (e.g., "C# 12", "Java 17")
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Release date
        /// </summary>
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// Associated framework/runtime version (e.g., ".NET 8" for C# 12)
        /// </summary>
        public string? FrameworkVersion { get; set; }

        /// <summary>
        /// Is this version still supported
        /// </summary>
        public bool IsSupported { get; set; } = true;

        /// <summary>
        /// Notable features introduced in this version
        /// </summary>
        public IList<string> Features { get; set; } = new List<string>();

        public ProgrammingLanguageVersion(string version, string displayName)
        {
            Version = version;
            DisplayName = displayName;
        }
    }
}
