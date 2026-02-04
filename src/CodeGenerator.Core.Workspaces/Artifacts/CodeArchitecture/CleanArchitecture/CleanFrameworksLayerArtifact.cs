using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture
{
    public class CleanFrameworksLayerArtifact : CodeArchitectureLayerArtifact
    {
        public CleanFrameworksLayerArtifact(string initialScopeName) : base(CleanCodeArchitecture.FRAMEWORKS_LAYER, initialScopeName)
        {
        }

        public CleanFrameworksLayerArtifact(ArtifactState state) : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("settings");
    }
}
