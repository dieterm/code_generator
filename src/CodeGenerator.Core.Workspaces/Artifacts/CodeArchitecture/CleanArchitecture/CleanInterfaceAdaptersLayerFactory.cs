using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture
{
    public class CleanInterfaceAdaptersLayerFactory : ICleanArchitectureLayerFactory
    {
        public string LayerName => CleanCodeArchitecture.INTERFACE_ADAPTERS_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            return new CleanInterfaceAdaptersLayerArtifact(scope);
        }
    }
}
