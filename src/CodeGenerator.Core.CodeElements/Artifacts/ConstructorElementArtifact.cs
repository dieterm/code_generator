using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class ConstructorElementArtifact : CodeElementArtifactBase<ConstructorElement>
{
    public ConstructorElementArtifact(ConstructorElement constructorElement) : base(constructorElement)
    {
        AddChild(new ParametersContainerArtifact(constructorElement.Parameters));
    }

    public ConstructorElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => $".ctor({string.Join(", ", CodeElement.Parameters.Select(p => p.Type.TypeName))})";

    public CompositeStatement Body => CodeElement.Body;

    public bool IsPrimary
    {
        get => CodeElement.IsPrimary;
        set => CodeElement.IsPrimary = value;
    }

    public bool IsStatic
    {
        get => CodeElement.IsStatic;
        set => CodeElement.IsStatic = value;
    }

    public ParametersContainerArtifact Parameters => Children.OfType<ParametersContainerArtifact>().Single();
}
