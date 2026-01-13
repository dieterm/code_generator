using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Models
{
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

        ///// <summary>
        ///// List of tables in the datasource
        ///// </summary>
        //[JsonPropertyName("tables")]
        //public List<TableDefinition> Tables { get; set; } = new();

        ///// <summary>
        ///// List of views in the datasource
        ///// </summary>
        //[JsonPropertyName("views")]
        //public List<ViewDefinition> Views { get; set; } = new();
    }
}
