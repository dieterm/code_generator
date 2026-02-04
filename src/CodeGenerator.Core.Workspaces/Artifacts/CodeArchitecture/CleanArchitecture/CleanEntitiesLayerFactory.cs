using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture
{
    public class CleanEntitiesLayerFactory : ICleanArchitectureLayerFactory
    {
        public string LayerName => CleanCodeArchitecture.ENTITIES_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            return new CleanEntitiesLayerArtifact(scope);
        }
    }
}
