using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class EnumMembersContainerArtifact : CodeElementArtifactBase, IEnumerable<EnumMemberElementArtifact>
{
    private readonly List<EnumMemberElement> _members;

    public EnumMembersContainerArtifact(List<EnumMemberElement> members) : base()
    {
        _members = members;
        foreach (var member in members)
            AddChild(new EnumMemberElementArtifact(member));
    }

    public EnumMembersContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Members";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public void AddNewMember()
    {
        var member = new EnumMemberElement("NewMember");
        _members.Add(member);
        AddChild(new EnumMemberElementArtifact(member));
    }

    public void RemoveMember(EnumMemberElementArtifact artifact)
    {
        _members.Remove(artifact.CodeElement);
        RemoveChild(artifact);
    }

    public IEnumerator<EnumMemberElementArtifact> GetEnumerator() => Children.OfType<EnumMemberElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
