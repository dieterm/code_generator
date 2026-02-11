using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class OperatorsContainerArtifact : CodeElementArtifactBase, IEnumerable<OperatorElementArtifact>
{
    private readonly List<OperatorElement> _operators;

    public OperatorsContainerArtifact(List<OperatorElement> operators) : base()
    {
        _operators = operators;
        foreach (var op in operators)
            AddChild(new OperatorElementArtifact(op));
    }

    public OperatorsContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Operators";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public void AddOperatorElement(OperatorElement operatorElement)
    {
        _operators.Add(operatorElement);
        AddChild(new OperatorElementArtifact(operatorElement));
    }

    public void RemoveOperatorElement(OperatorElementArtifact operatorElementArtifact)
    {
        RemoveChild(operatorElementArtifact);
        _operators.Remove(operatorElementArtifact.CodeElement);
    }

    public IEnumerator<OperatorElementArtifact> GetEnumerator() => Children.OfType<OperatorElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
