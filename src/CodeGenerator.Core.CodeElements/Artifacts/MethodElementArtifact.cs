using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class MethodElementArtifact : CodeElementArtifactBase<MethodElement>
{
    public MethodElementArtifact(MethodElement methodElement) : base(methodElement)
    {
        AddChild(new ParametersContainerArtifact(methodElement.Parameters));
    }

    public MethodElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public string ReturnTypeName
    {
        get => CodeElement.ReturnType.TypeName;
        set => CodeElement.ReturnType.TypeName = value;
    }

    public CompositeStatement Body => CodeElement.Body;

    public bool IsExpressionBodied
    {
        get => CodeElement.IsExpressionBodied;
        set => CodeElement.IsExpressionBodied = value;
    }

    public string? ExpressionBody
    {
        get => CodeElement.ExpressionBody;
        set => CodeElement.ExpressionBody = value;
    }

    public bool IsExtensionMethod
    {
        get => CodeElement.IsExtensionMethod;
        set => CodeElement.IsExtensionMethod = value;
    }

    public ParametersContainerArtifact Parameters => Children.OfType<ParametersContainerArtifact>().Single();
}
