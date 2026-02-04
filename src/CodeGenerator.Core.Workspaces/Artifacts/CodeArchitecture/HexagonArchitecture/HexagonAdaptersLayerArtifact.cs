using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.HexagonArchitecture
{
    public class HexagonAdaptersLayerArtifact : CodeArchitectureLayerArtifact
    {
        public HexagonAdaptersLayerArtifact(string initialScopeName) : base(HexagonCodeArchitecture.ADAPTERS_LAYER, initialScopeName)
        {
        }

        public HexagonAdaptersLayerArtifact(ArtifactState state) : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("cable");
    }
}
