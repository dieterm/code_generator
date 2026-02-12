using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class EnumElementArtifact : CodeElementArtifactBase<EnumElement>
{
    public EnumElementArtifact(EnumElement enumElement) : base(enumElement)
    {
        AddChild(new EnumMembersContainerArtifact(enumElement.Members));
    }

    public EnumElementArtifact(ArtifactState artifactState) : base(artifactState) { }

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
