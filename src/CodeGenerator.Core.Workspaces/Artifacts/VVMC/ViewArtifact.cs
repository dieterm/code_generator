using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture;

namespace CodeGenerator.Core.Workspaces.Artifacts.VVMC
{
    public class ViewArtifact : CodeArchitectureLayerChildArtifact
    {
        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("app-window");

        public ViewArtifact(string viewName)
        {
            Name = viewName;
        }

        public ViewArtifact(ArtifactState state)
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
