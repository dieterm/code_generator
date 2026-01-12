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

}
