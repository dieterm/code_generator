using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.HexagonArchitecture
{
    public class HexagonPortsLayerArtifact : CodeArchitectureLayerArtifact
    {
        public HexagonPortsLayerArtifact(string initialScopeName) : base(HexagonCodeArchitecture.PORTS_LAYER, initialScopeName)
        {
        }

        public HexagonPortsLayerArtifact(ArtifactState state) : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("plug");
    }
}
