using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture
{
    public class CleanUseCasesLayerArtifact : CodeArchitectureLayerArtifact
    {
        public CleanUseCasesLayerArtifact(string initialScopeName) : base(CleanCodeArchitecture.USE_CASES_LAYER, initialScopeName)
        {
        }

        public CleanUseCasesLayerArtifact(ArtifactState state) : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("workflow");
    }
}
