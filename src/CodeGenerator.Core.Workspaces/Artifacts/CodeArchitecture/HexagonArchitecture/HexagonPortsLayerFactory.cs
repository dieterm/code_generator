using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.HexagonArchitecture
{
    public class HexagonPortsLayerFactory : IHexagonArchitectureLayerFactory
    {
        public string LayerName => HexagonCodeArchitecture.PORTS_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            return new HexagonPortsLayerArtifact(scope);
        }
    }
}
