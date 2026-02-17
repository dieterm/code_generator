using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class GenericConstraintsContainerArtifact : CodeElementArtifactBase, IEnumerable<GenericConstraintElementArtifact>
{
    private readonly List<GenericConstraintElement> _genericConstraints;

    public GenericConstraintsContainerArtifact(List<GenericConstraintElement> genericConstraints) : base()
    {
        _genericConstraints = genericConstraints;
        foreach (var constraint in genericConstraints)
            AddChild(new GenericConstraintElementArtifact(constraint));
    }

    public GenericConstraintsContainerArtifact(ArtifactState artifactState, List<string> errors) : base(artifactState, errors) { }

    public override string TreeNodeText => "Generic Constraints";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("code");

    public void AddNewConstraint()
    {
        var constraint = new GenericConstraintElement("T");
        _genericConstraints.Add(constraint);
        AddChild(new GenericConstraintElementArtifact(constraint));
    }

    public void RemoveConstraint(GenericConstraintElementArtifact artifact)
    {
        _genericConstraints.Remove(artifact.CodeElement);
        RemoveChild(artifact);
    }

    public IEnumerator<GenericConstraintElementArtifact> GetEnumerator() => Children.OfType<GenericConstraintElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
