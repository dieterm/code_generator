using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class FieldElementArtifact : CodeElementArtifactBase<FieldElement>
{
    public FieldElementArtifact(FieldElement fieldElement) : base(fieldElement) { }
    public FieldElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public string TypeName
    {
        get => CodeElement.Type.TypeName;
        set => CodeElement.Type.TypeName = value;
    }

    public string? InitialValue
    {
        get => CodeElement.InitialValue;
        set => CodeElement.InitialValue = value;
    }
}
