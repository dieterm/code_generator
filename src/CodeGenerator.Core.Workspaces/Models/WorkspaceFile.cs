using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Workspaces.Models
{
    /// <summary>
    /// Model representing the .codegenerator workspace file content
    /// </summary>
    public class WorkspaceFile
    {
        /// <summary>
        /// File format version for backwards compatibility
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Workspace name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Workspace";

        /// <summary>
        /// Root namespace for generated code
        /// </summary>
        [JsonPropertyName("rootNamespace")]
        public string RootNamespace { get; set; } = "MyCompany.MyProduct";

        /// <summary>
        /// Default output directory (relative to workspace folder)
        /// </summary>
        [JsonPropertyName("defaultOutputDirectory")]
        public string DefaultOutputDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Default target framework
        /// </summary>
        [JsonPropertyName("defaultTargetFramework")]
        public string DefaultTargetFramework { get; set; } = "net8.0";

        /// <summary>
        /// Default programming language
        /// </summary>
        [JsonPropertyName("defaultLanguage")]
        public string DefaultLanguage { get; set; } = "C#";

        /// <summary>
        /// List of datasources in the workspace
        /// </summary>
        [JsonPropertyName("datasources")]
        public List<DatasourceDefinition> Datasources { get; set; } = new();
    }

    /// <summary>
    /// Base definition for a datasource in the workspace file
    /// </summary>
    public class DatasourceDefinition
    {
        /// <summary>
        /// Unique identifier for the datasource
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Display name of the datasource
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type of datasource (e.g., "SqlServer", "MySql", "Json", etc.)
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Category of datasource (e.g., "RelationalDatabase", "NonRelationalDatabase", "File")
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Description of the datasource
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Connection string (for database datasources)
        /// </summary>
        [JsonPropertyName("connectionString")]
        public string? ConnectionString { get; set; }

        /// <summary>
        /// File path (for file-based datasources)
        /// </summary>
        [JsonPropertyName("filePath")]
        public string? FilePath { get; set; }

        /// <summary>
        /// Additional settings specific to the datasource type
        /// </summary>
        [JsonPropertyName("settings")]
        public Dictionary<string, object>? Settings { get; set; }
    }
}
