using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class ConstructorsContainerArtifact : CodeElementArtifactBase, IEnumerable<ConstructorElementArtifact>
{
    private readonly List<ConstructorElement> _constructors;

    public ConstructorsContainerArtifact(List<ConstructorElement> constructors) : base()
    {
        _constructors = constructors;
        foreach (var ctor in constructors)
            AddChild(new ConstructorElementArtifact(ctor));
    }

    public ConstructorsContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Constructors";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public void AddNewConstructor()
    {
        var ctor = new ConstructorElement();
        _constructors.Add(ctor);
        AddChild(new ConstructorElementArtifact(ctor));
    }

    public void RemoveConstructor(ConstructorElementArtifact artifact)
    {
        _constructors.Remove(artifact.CodeElement);
        RemoveChild(artifact);
    }

    public IEnumerator<ConstructorElementArtifact> GetEnumerator() => Children.OfType<ConstructorElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
