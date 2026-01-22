namespace CodeGenerator.Domain.Databases
{
    /// <summary>
    /// Represents a specific version of a database
    /// </summary>
    public class DatabaseVersion
    {
        /// <summary>
        /// Version identifier (e.g., "8.0", "15", "2022")
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Display name (e.g., "MySQL 8.0", "PostgreSQL 15")
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Release date
        /// </summary>
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// Is this version still supported
        /// </summary>
        public bool IsSupported { get; set; } = true;

        /// <summary>
        /// Notable features introduced in this version
        /// </summary>
        public IList<string> Features { get; set; } = new List<string>();

        public DatabaseVersion(string version, string displayName)
        {
            Version = version;
            DisplayName = displayName;
        }
    }
}
