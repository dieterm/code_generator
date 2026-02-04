using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.NTierArchitecture
{
    public class NTierBusinessLayerFactory : INTierArchitectureLayerFactory
    {
        public string LayerName => NTierCodeArchitecture.BUSINESS_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            return new NTierBusinessLayerArtifact(scope);
        }
    }
}
