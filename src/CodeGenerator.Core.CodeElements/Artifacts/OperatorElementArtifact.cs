using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class OperatorElementArtifact : CodeElementArtifactBase<OperatorElement>
{
    public OperatorElementArtifact(OperatorElement operatorElement) : base(operatorElement)
    {
        AddChild(new ParametersContainerArtifact(operatorElement.Parameters));
    }

    public OperatorElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => $"operator {CodeElement.OperatorType}";

    public OperatorType OperatorType
    {
        get => CodeElement.OperatorType;
        set => CodeElement.OperatorType = value;
    }

    public string ReturnTypeName
    {
        get => CodeElement.ReturnType.TypeName;
        set => CodeElement.ReturnType.TypeName = value;
    }

    public CompositeStatement Body => CodeElement.Body;

    public ParametersContainerArtifact Parameters => Children.OfType<ParametersContainerArtifact>().Single();
}
