using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class ConstructorElementArtifact : CodeElementArtifactBase<ConstructorElement>
{
    public ConstructorElementArtifact(ConstructorElement constructorElement) : base(constructorElement)
    {
        AddChild(new ParametersContainerArtifact(constructorElement.Parameters));
        AddChild(new CompositeStatementArtifact(constructorElement.Body, true) { Name = nameof(Body) });
    }

    public ConstructorElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => $".ctor({string.Join(", ", CodeElement.Parameters.Select(p => p.Type.TypeName))})";

    public CompositeStatement Body => CodeElement.Body;

    public bool IsPrimary
    {
        get { return CodeElement.IsPrimary; }
        set
        {
            if (CodeElement.IsPrimary != value)
            {
                CodeElement.IsPrimary = value;
                RaisePropertyChangedEvent(nameof(IsPrimary));
            }
        }
    }

    public bool IsStatic
    {
        get { return CodeElement.IsStatic; }
        set
        {
            if (CodeElement.IsStatic != value)
            {
                CodeElement.IsStatic = value;
                RaisePropertyChangedEvent(nameof(IsStatic));
            }
        }
    }

    public ParametersContainerArtifact Parameters => Children.OfType<ParametersContainerArtifact>().Single();
}
