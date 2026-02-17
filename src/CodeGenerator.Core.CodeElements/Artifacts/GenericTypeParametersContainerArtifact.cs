using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class GenericTypeParametersContainerArtifact : CodeElementArtifactBase, IEnumerable<GenericTypeParameterElementArtifact>
{
    private readonly List<GenericTypeParameterElement> _genericTypeParameters;

    public GenericTypeParametersContainerArtifact(List<GenericTypeParameterElement> genericTypeParameters) : base()
    {
        _genericTypeParameters = genericTypeParameters;
        foreach (var parameter in genericTypeParameters)
            AddChild(new GenericTypeParameterElementArtifact(parameter));
    }

    public GenericTypeParametersContainerArtifact(ArtifactState artifactState, List<string> errors) : base(artifactState, errors) { }

    public override string TreeNodeText => "Generic Type Parameters";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("code");

    public void AddNewParameter()
    {
        var parameter = new GenericTypeParameterElement("T");
        _genericTypeParameters.Add(parameter);
        AddChild(new GenericTypeParameterElementArtifact(parameter));
    }

    public void RemoveParameter(GenericTypeParameterElementArtifact artifact)
    {
        _genericTypeParameters.Remove(artifact.CodeElement);
        RemoveChild(artifact);
    }

    public IEnumerator<GenericTypeParameterElementArtifact> GetEnumerator() => Children.OfType<GenericTypeParameterElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
