using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class EnumElementArtifact : TypeElementArtifactBase<EnumElement>
{
    public EnumElementArtifact(EnumElement enumElement) : base(enumElement)
    {
        AddChild(new EnumMembersContainerArtifact(enumElement.Members));
    }

    public EnumElementArtifact(ArtifactState artifactState, List<string> errors) : base(artifactState, errors) { }

    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("braces");

    public bool IsFlags
    {
        get { return CodeElement.IsFlags; }
        set
        {
            if (CodeElement.IsFlags != value)
            {
                CodeElement.IsFlags = value;
                RaisePropertyChangedEvent(nameof(IsFlags));
            }
        }
    }

    public EnumMembersContainerArtifact Members => Children.OfType<EnumMembersContainerArtifact>().Single();
}
