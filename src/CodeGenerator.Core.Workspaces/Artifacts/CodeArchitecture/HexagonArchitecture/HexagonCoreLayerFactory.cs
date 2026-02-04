using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.HexagonArchitecture
{
    public class HexagonCoreLayerFactory : IHexagonArchitectureLayerFactory
    {
        public string LayerName => HexagonCodeArchitecture.CORE_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            return new HexagonCoreLayerArtifact(scope);
        }
    }
}
