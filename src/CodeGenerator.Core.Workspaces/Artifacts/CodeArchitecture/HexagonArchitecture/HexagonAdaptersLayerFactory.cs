using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.HexagonArchitecture
{
    public class HexagonAdaptersLayerFactory : IHexagonArchitectureLayerFactory
    {
        public string LayerName => HexagonCodeArchitecture.ADAPTERS_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            return new HexagonAdaptersLayerArtifact(scope);
        }
    }
}
