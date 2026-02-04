using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.VVMC
{
    public class ViewModelsContainerArtifact : CodeArchitectureLayerChildArtifact
    {
        public override string TreeNodeText => "ViewModels";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("component");

        public ViewModelsContainerArtifact()
        {
        }

        public ViewModelsContainerArtifact(ArtifactState state)
            : base(state)
        {
        }
    }
}
