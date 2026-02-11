using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class MethodsContainerArtifact : CodeElementArtifactBase, IEnumerable<MethodElementArtifact>
{
    private readonly List<MethodElement> _methods;

    public MethodsContainerArtifact(List<MethodElement> methods) : base()
    {
        _methods = methods;
        foreach (var method in methods)
            AddChild(new MethodElementArtifact(method));
    }

    public MethodsContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Methods";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public void AddNewMethod()
    {
        var method = new MethodElement("NewMethod");
        _methods.Add(method);
        AddChild(new MethodElementArtifact(method));
    }

    public void RemoveMethod(MethodElementArtifact artifact)
    {
        _methods.Remove(artifact.CodeElement);
        RemoveChild(artifact);
    }

    public IEnumerator<MethodElementArtifact> GetEnumerator() => Children.OfType<MethodElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
