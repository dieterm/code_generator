using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture
{
    public class CleanUseCasesLayerFactory : ICleanArchitectureLayerFactory
    {
        public string LayerName => CleanCodeArchitecture.USE_CASES_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            return new CleanUseCasesLayerArtifact(scope);
        }
    }
}
