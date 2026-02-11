using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class ParametersContainerArtifact : CodeElementArtifactBase, IEnumerable<ParameterElementArtifact>
{
    private readonly List<ParameterElement> _parameters;

    public ParametersContainerArtifact(List<ParameterElement> parameters) : base()
    {
        _parameters = parameters;
        foreach (var param in parameters)
            AddChild(new ParameterElementArtifact(param));
    }

    public ParametersContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Parameters";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public void AddNewParameter()
    {
        var param = new ParameterElement("newParam", new TypeReference("string"));
        _parameters.Add(param);
        AddChild(new ParameterElementArtifact(param));
    }

    public void RemoveParameter(ParameterElementArtifact artifact)
    {
        _parameters.Remove(artifact.CodeElement);
        RemoveChild(artifact);
    }

    public IEnumerator<ParameterElementArtifact> GetEnumerator() => Children.OfType<ParameterElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
