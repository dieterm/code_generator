using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.NTierArchitecture
{
    public class NTierDataAccessLayerArtifact : CodeArchitectureLayerArtifact
    {
        public NTierDataAccessLayerArtifact(string initialScopeName) : base(NTierCodeArchitecture.DATA_ACCESS_LAYER, initialScopeName)
        {
        }

        public NTierDataAccessLayerArtifact(ArtifactState state) : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("database");
    }
}
