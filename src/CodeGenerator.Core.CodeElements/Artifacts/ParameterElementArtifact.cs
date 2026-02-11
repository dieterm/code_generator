using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class ParameterElementArtifact : CodeElementArtifactBase<ParameterElement>
{
    public ParameterElementArtifact(ParameterElement parameterElement) : base(parameterElement) { }
    public ParameterElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public string TypeName
    {
        get => CodeElement.Type.TypeName;
        set => CodeElement.Type.TypeName = value;
    }

    public ParameterModifier Modifier
    {
        get => CodeElement.Modifier;
        set => CodeElement.Modifier = value;
    }

    public string? DefaultValue
    {
        get => CodeElement.DefaultValue;
        set => CodeElement.DefaultValue = value;
    }

    public bool IsExtensionMethodThis
    {
        get => CodeElement.IsExtensionMethodThis;
        set => CodeElement.IsExtensionMethodThis = value;
    }
}
