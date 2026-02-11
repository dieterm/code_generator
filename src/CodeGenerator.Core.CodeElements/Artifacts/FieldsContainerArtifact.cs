using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class FieldsContainerArtifact : CodeElementArtifactBase, IEnumerable<FieldElementArtifact>
{
    private readonly List<FieldElement> _fields;

    public FieldsContainerArtifact(List<FieldElement> fields) : base()
    {
        _fields = fields;
        foreach (var field in fields)
            AddChild(new FieldElementArtifact(field));
    }

    public FieldsContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Fields";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public void AddNewField()
    {
        var field = new FieldElement("newField", new TypeReference("string"));
        _fields.Add(field);
        AddChild(new FieldElementArtifact(field));
    }

    public void RemoveField(FieldElementArtifact artifact)
    {
        _fields.Remove(artifact.CodeElement);
        RemoveChild(artifact);
    }

    public IEnumerator<FieldElementArtifact> GetEnumerator() => Children.OfType<FieldElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
