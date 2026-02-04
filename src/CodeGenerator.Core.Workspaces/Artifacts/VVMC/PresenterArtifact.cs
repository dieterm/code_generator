using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.VVMC
{
    public class PresenterArtifact : CodeArchitectureLayerChildArtifact
    {
        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("user");

        public PresenterArtifact(string presenterName)
        {
            Name = presenterName;
        }

        public PresenterArtifact(ArtifactState state)
            : base(state)
        {
        }

        public string? Name
        {
            get { return GetValue<string>(nameof(Name)); }
            set
            {
                if (SetValue<string>(nameof(Name), value))
                {
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                }
            }
        }
    }
}
