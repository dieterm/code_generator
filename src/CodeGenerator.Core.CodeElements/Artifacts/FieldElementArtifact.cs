using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class FieldElementArtifact : CodeElementArtifactBase<FieldElement>
{
    public FieldElementArtifact(FieldElement fieldElement) : base(fieldElement) { }
    public FieldElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public string TypeName
    {
        get { return CodeElement.Type.TypeName; }
        set
        {
            if (CodeElement.Type.TypeName != value)
            {
                CodeElement.Type.TypeName = value;
                RaisePropertyChangedEvent(nameof(TypeName));
            }
        }
    }

    public string? InitialValue
    {
        get { return CodeElement.InitialValue; }
        set
        {
            if (CodeElement.InitialValue != value)
            {
                CodeElement.InitialValue = value;
                RaisePropertyChangedEvent(nameof(InitialValue));
            }
        }
    }
}
