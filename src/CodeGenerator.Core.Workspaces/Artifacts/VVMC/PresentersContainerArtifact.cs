using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.VVMC
{
    public class PresentersContainerArtifact : CodeArchitectureLayerChildArtifact
    {
        public override string TreeNodeText => "Presenters";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("presentation");

        public PresentersContainerArtifact()
        {
        }

        public PresentersContainerArtifact(ArtifactState state)
            : base(state)
        {
        }
    }
}
