using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture
{
    public class CleanInterfaceAdaptersLayerArtifact : CodeArchitectureLayerArtifact
    {
        public CleanInterfaceAdaptersLayerArtifact(string initialScopeName) : base(CleanCodeArchitecture.INTERFACE_ADAPTERS_LAYER, initialScopeName)
        {
        }

        public CleanInterfaceAdaptersLayerArtifact(ArtifactState state, List<string> errors) : base(state, errors)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("arrow-left-right");
    }
}
