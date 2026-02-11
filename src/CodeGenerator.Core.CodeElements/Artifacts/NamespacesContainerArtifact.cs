using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class NamespacesContainerArtifact : CodeElementArtifactBase<CodeFileElement>, IEnumerable<NamespaceElementArtifact>
{
    public NamespacesContainerArtifact(CodeFileElement codeFileElement  ) : base(codeFileElement)
    {
        foreach (var ns in CodeElement.Namespaces)
        {
            AddChild(new NamespaceElementArtifact(ns));
        }
    }

    public NamespacesContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Namespaces";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public void AddNewNamespace()
    {
        var ns = new NamespaceElement("MyNamespace");
        CodeElement.Namespaces.Add(ns);
        AddChild(new NamespaceElementArtifact(ns));
    }

    public void RemoveNamespace(NamespaceElementArtifact artifact)
    {
        CodeElement.Namespaces.Remove(artifact.CodeElement);
        RemoveChild(artifact);
    }

    public IEnumerator<NamespaceElementArtifact> GetEnumerator() => Children.OfType<NamespaceElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
