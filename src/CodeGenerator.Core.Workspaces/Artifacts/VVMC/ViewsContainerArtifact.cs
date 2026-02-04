using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.VVMC
{
    public class ViewsContainerArtifact : CodeArchitectureLayerChildArtifact
    {
        public override string TreeNodeText => "Views";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("layout-dashboard");

        public ViewsContainerArtifact()
        {
            
        }

        public ViewsContainerArtifact(ArtifactState state)
            : base(state)
        {
        }
    }
}
