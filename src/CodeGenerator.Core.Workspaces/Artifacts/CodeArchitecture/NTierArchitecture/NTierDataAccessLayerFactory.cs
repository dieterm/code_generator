using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.NTierArchitecture
{
    public class NTierDataAccessLayerFactory : INTierArchitectureLayerFactory
    {
        public string LayerName => NTierCodeArchitecture.DATA_ACCESS_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            return new NTierDataAccessLayerArtifact(scope);
        }
    }
}
