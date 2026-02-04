using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture
{
    public class CleanEntitiesLayerArtifact : CodeArchitectureLayerArtifact
    {
        public CleanEntitiesLayerArtifact(string initialScopeName) : base(CleanCodeArchitecture.ENTITIES_LAYER, initialScopeName)
        {
        }

        public CleanEntitiesLayerArtifact(ArtifactState state) : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("box");
    }
}
