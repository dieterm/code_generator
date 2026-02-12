using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class OperatorElementArtifact : CodeElementArtifactBase<OperatorElement>
{
    public OperatorElementArtifact(OperatorElement operatorElement) : base(operatorElement)
    {
        AddChild(new ParametersContainerArtifact(operatorElement.Parameters));
        AddChild(new CompositeStatementArtifact(operatorElement.Body, true) { Name = nameof(Body) });
    }

    public OperatorElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => $"operator {CodeElement.OperatorType}";

    public OperatorType OperatorType
    {
        get { return CodeElement.OperatorType; }
        set
        {
            if (CodeElement.OperatorType != value)
            {
                CodeElement.OperatorType = value;
                RaisePropertyChangedEvent(nameof(OperatorType));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }
    }

    public string ReturnTypeName
    {
        get { return CodeElement.ReturnType.TypeName; }
        set
        {
            if (CodeElement.ReturnType.TypeName != value)
            {
                CodeElement.ReturnType.TypeName = value;
                RaisePropertyChangedEvent(nameof(ReturnTypeName));
            }
        }
    }

    public CompositeStatement Body => CodeElement.Body;

    public ParametersContainerArtifact Parameters => Children.OfType<ParametersContainerArtifact>().Single();
}
