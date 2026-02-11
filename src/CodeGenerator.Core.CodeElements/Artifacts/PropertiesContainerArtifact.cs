using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class PropertiesContainerArtifact : CodeElementArtifactBase, IEnumerable<PropertyElementArtifact>
{
    private readonly List<PropertyElement> _properties;

    public PropertiesContainerArtifact(List<PropertyElement> properties) : base()
    {
        _properties = properties;
        foreach (var prop in properties)
            AddChild(new PropertyElementArtifact(prop));
    }

    public PropertiesContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Properties";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public void AddNewProperty()
    {
        var prop = new PropertyElement("NewProperty", new TypeReference("string"));
        _properties.Add(prop);
        AddChild(new PropertyElementArtifact(prop));
    }

    public void RemoveProperty(PropertyElementArtifact artifact)
    {
        _properties.Remove(artifact.CodeElement);
        RemoveChild(artifact);
    }

    public IEnumerator<PropertyElementArtifact> GetEnumerator() => Children.OfType<PropertyElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
