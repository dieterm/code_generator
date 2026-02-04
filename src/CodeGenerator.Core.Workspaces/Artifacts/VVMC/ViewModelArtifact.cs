using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.VVMC
{
    public class ViewModelArtifact : CodeArchitectureLayerChildArtifact
    {
        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("file-code");

        public ViewModelArtifact(string viewModelName)
        {
            Name = viewModelName;
        }

        public ViewModelArtifact(ArtifactState state)
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
