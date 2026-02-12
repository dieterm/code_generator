using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class MethodElementArtifact : CodeElementArtifactBase<MethodElement>
{
    public MethodElementArtifact(MethodElement methodElement) : base(methodElement)
    {
        AddChild(new ParametersContainerArtifact(methodElement.Parameters));
        AddChild(new CompositeStatementArtifact(methodElement.Body, true) { Name = nameof(Body) });
    }

    public MethodElementArtifact(ArtifactState artifactState) : base(artifactState) { }

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

    public bool IsExpressionBodied
    {
        get { return CodeElement.IsExpressionBodied; }
        set
        {
            if (CodeElement.IsExpressionBodied != value)
            {
                CodeElement.IsExpressionBodied = value;
                RaisePropertyChangedEvent(nameof(IsExpressionBodied));
            }
        }
    }

    public string? ExpressionBody
    {
        get { return CodeElement.ExpressionBody; }
        set
        {
            if (CodeElement.ExpressionBody != value)
            {
                CodeElement.ExpressionBody = value;
                RaisePropertyChangedEvent(nameof(ExpressionBody));
            }
        }
    }

    public bool IsExtensionMethod
    {
        get { return CodeElement.IsExtensionMethod; }
        set
        {
            if (CodeElement.IsExtensionMethod != value)
            {
                CodeElement.IsExtensionMethod = value;
                RaisePropertyChangedEvent(nameof(IsExtensionMethod));
            }
        }
    }

    public ParametersContainerArtifact Parameters => Children.OfType<ParametersContainerArtifact>().Single();
}
