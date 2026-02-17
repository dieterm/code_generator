using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class BaseTypesContainerArtifact : CodeElementArtifactBase, IEnumerable<TypeReferenceArtifact>
{
    private readonly List<TypeReference> _baseTypes;

    public BaseTypesContainerArtifact(List<TypeReference> baseTypes) : base()
    {
        _baseTypes = baseTypes;
        foreach (var baseType in baseTypes)
            AddChild(new TypeReferenceArtifact(baseType));
    }

    public BaseTypesContainerArtifact(ArtifactState artifactState, List<string> errors) : base(artifactState, errors) { }

    public override string TreeNodeText => "Base Types";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("braces");

    public void AddNewBaseType()
    {
        var baseType = new TypeReference("object");
        _baseTypes.Add(baseType);
        AddChild(new TypeReferenceArtifact(baseType));
    }

    public void RemoveBaseType(TypeReferenceArtifact artifact)
    {
        _baseTypes.Remove(artifact.TypeReference);
        RemoveChild(artifact);
    }

    public IEnumerator<TypeReferenceArtifact> GetEnumerator() => Children.OfType<TypeReferenceArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
