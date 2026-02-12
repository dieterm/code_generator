using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class ParameterElementArtifact : CodeElementArtifactBase<ParameterElement>
{
    public ParameterElementArtifact(ParameterElement parameterElement) : base(parameterElement) { }
    public ParameterElementArtifact(ArtifactState artifactState) : base(artifactState) { }

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

    public ParameterModifier Modifier
    {
        get { return CodeElement.Modifier; }
        set
        {
            if (CodeElement.Modifier != value)
            {
                CodeElement.Modifier = value;
                RaisePropertyChangedEvent(nameof(Modifier));
            }
        }
    }

    public string? DefaultValue
    {
        get { return CodeElement.DefaultValue; }
        set
        {
            if (CodeElement.DefaultValue != value)
            {
                CodeElement.DefaultValue = value;
                RaisePropertyChangedEvent(nameof(DefaultValue));
            }
        }
    }

    public bool IsExtensionMethodThis
    {
        get { return CodeElement.IsExtensionMethodThis; }
        set
        {
            if (CodeElement.IsExtensionMethodThis != value)
            {
                CodeElement.IsExtensionMethodThis = value;
                RaisePropertyChangedEvent(nameof(IsExtensionMethodThis));
            }
        }
    }
}
