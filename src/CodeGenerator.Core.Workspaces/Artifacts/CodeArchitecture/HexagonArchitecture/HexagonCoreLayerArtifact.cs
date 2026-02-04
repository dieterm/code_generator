using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.HexagonArchitecture
{
    public class HexagonCoreLayerArtifact : CodeArchitectureLayerArtifact
    {
        public HexagonCoreLayerArtifact(string initialScopeName) : base(HexagonCodeArchitecture.CORE_LAYER, initialScopeName)
        {
        }

        public HexagonCoreLayerArtifact(ArtifactState state) : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("hexagon");
    }
}
