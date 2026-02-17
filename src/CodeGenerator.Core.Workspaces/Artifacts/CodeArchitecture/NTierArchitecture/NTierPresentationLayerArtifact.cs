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

        public NTierPresentationLayerArtifact(ArtifactState state, List<string> errors) : base(state, errors)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("monitor");
    }
}
