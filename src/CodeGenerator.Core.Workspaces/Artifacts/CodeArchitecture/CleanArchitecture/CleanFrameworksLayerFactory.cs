using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture
{
    public class CleanFrameworksLayerFactory : ICleanArchitectureLayerFactory
    {
        public string LayerName => CleanCodeArchitecture.FRAMEWORKS_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            return new CleanFrameworksLayerArtifact(scope);
        }
    }
}
