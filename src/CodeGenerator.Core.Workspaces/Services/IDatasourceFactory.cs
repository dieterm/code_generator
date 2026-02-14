using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
//using CodeGenerator.Core.Workspaces.Models;

namespace CodeGenerator.Core.Workspaces.Services
{
    /// <summary>
    /// Factory interface for creating datasource artifacts from definitions
    /// </summary>
    public interface IDatasourceFactory
    {
        /// <summary>
        /// Create a datasource artifact from a definition
        /// </summary>
        DatasourceArtifact? Create(DatasourceDefinition definition);

        /// <summary>
        /// Create a definition from a datasource artifact
        /// </summary>
        DatasourceDefinition? CreateDefinition(DatasourceArtifact datasource);

        /// <summary>
        /// Get all registered datasource types
        /// </summary>
        IEnumerable<DatasourceTypeInfo> GetAvailableTypes();

        /// <summary>
        /// Get a specific provider by type identifier
        /// </summary>
        IDatasourceProvider? GetProvider(string typeId);
        void RegisterProvider(IDatasourceProvider provider);
        void UnregisterProvider(string typeId);
    }
}
