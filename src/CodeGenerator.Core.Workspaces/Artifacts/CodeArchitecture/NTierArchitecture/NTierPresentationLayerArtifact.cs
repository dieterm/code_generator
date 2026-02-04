using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.NTierArchitecture
{
    public class NTierPresentationLayerArtifact : CodeArchitectureLayerArtifact
    {
        public NTierPresentationLayerArtifact(string initialScopeName) : base(NTierCodeArchitecture.PRESENTATION_LAYER, initialScopeName)
        {
        }

        public NTierPresentationLayerArtifact(ArtifactState state) : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("monitor");
    }
}
