using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Workspaces.Services
{
    /// <summary>
    /// Default implementation of the datasource factory
    /// Uses registered providers to create datasources
    /// </summary>
    public class DatasourceFactory : IDatasourceFactory
    {
        private readonly ILogger<DatasourceFactory> _logger;
        private readonly Dictionary<string, IDatasourceProvider> _providers = new(StringComparer.OrdinalIgnoreCase);

        public DatasourceFactory(ILogger<DatasourceFactory> logger, IEnumerable<IDatasourceProvider> providers)
        {
            _logger = logger;
            foreach (var provider in providers)
            {
                RegisterProvider(provider);
            }
        }

        /// <summary>
        /// Register a datasource provider
        /// </summary>
        public void RegisterProvider(IDatasourceProvider provider)
        {
            _providers[provider.TypeId] = provider;
            _logger.LogDebug("Registered datasource provider: {TypeId}", provider.TypeId);
        }

        /// <summary>
        /// Unregister a datasource provider
        /// </summary>
        public void UnregisterProvider(string typeId)
        {
            _providers.Remove(typeId);
        }

        public DatasourceArtifact? Create(DatasourceDefinition definition)
        {
            if (_providers.TryGetValue(definition.Type, out var provider))
            {
                return provider.CreateFromDefinition(definition);
            }

            _logger.LogWarning("No provider registered for datasource type: {Type}", definition.Type);
            return null;
        }

        public DatasourceDefinition? CreateDefinition(DatasourceArtifact datasource)
        {
            if (_providers.TryGetValue(datasource.DatasourceType, out var provider))
            {
                return provider.CreateDefinition(datasource);
            }

            _logger.LogWarning("No provider registered for datasource type: {Type}", datasource.DatasourceType);
            return null;
        }

        public IEnumerable<DatasourceTypeInfo> GetAvailableTypes()
        {
            return _providers.Values.Select(p => p.GetTypeInfo());
        }

        public IDatasourceProvider GetProvider(string typeId)
        {
            if(_providers.TryGetValue(typeId, out var provider))
                            {
                return provider;
            } else
            {
                return null;
            }
        }
    }
}
