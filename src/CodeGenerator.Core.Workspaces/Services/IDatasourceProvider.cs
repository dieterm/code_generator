using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Domain.DataTypes;
//using CodeGenerator.Core.Workspaces.Models;

namespace CodeGenerator.Core.Workspaces.Services
{
    /// <summary>
    /// Interface for datasource providers
    /// Each datasource type (SqlServer, MySql, Json, etc.) implements this interface
    /// </summary>
    public interface IDatasourceProvider
    {
        /// <summary>
        /// Unique type identifier (e.g., "SqlServer", "MySql", "Json")
        /// </summary>
        string TypeId { get; }

        /// <summary>
        /// Get type information for this provider
        /// </summary>
        DatasourceTypeInfo GetTypeInfo();

        /// <summary>
        /// Create a datasource artifact from a definition
        /// </summary>
        DatasourceArtifact CreateFromDefinition(DatasourceDefinition definition);

        /// <summary>
        /// Create a new empty datasource with default settings
        /// </summary>
        DatasourceArtifact CreateNew(string name);

        /// <summary>
        /// Create a definition from a datasource artifact
        /// </summary>
        DatasourceDefinition CreateDefinition(DatasourceArtifact datasource);

        IEnumerable<GenericDataType> GetSupportedColumnDataTypes();
    }
}
