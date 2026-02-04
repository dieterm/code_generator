using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.NTierArchitecture
{
    public class NTierBusinessLayerArtifact : CodeArchitectureLayerArtifact
    {
        public NTierBusinessLayerArtifact(string initialScopeName) : base(NTierCodeArchitecture.BUSINESS_LAYER, initialScopeName)
        {
        }

        public NTierBusinessLayerArtifact(ArtifactState state) : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("briefcase");
    }
}
