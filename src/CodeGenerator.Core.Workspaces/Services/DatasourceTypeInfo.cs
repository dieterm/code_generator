//using CodeGenerator.Core.Workspaces.Models;

namespace CodeGenerator.Core.Workspaces.Services
{
    /// <summary>
    /// Information about a registered datasource type
    /// </summary>
    public class DatasourceTypeInfo
    {
        /// <summary>
        /// Type identifier (e.g., "SqlServer", "MySql", "Json")
        /// </summary>
        public string TypeId { get; init; } = string.Empty;

        /// <summary>
        /// Display name for the type
        /// </summary>
        public string DisplayName { get; init; } = string.Empty;

        /// <summary>
        /// Category (e.g., "Relational Database", "Non-Relational Database", "File")
        /// </summary>
        public DatasourceCategory Category { get; init; } = DatasourceCategory.File;

        /// <summary>
        /// Description of the datasource type
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Icon key for the datasource type
        /// </summary>
        public string IconKey { get; init; } = "database";

        /// <summary>
        /// File picker filter for the datasource type (used for file datasources)
        /// </summary>
        public string? FilePickerFilter { get; init; }
    }
}
