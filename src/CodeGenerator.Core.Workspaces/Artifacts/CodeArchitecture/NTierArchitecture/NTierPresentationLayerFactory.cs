using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.NTierArchitecture
{
    public class NTierPresentationLayerFactory : INTierArchitectureLayerFactory
    {
        public string LayerName => NTierCodeArchitecture.PRESENTATION_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            return new NTierPresentationLayerArtifact(scope);
        }
    }
}
