using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class EnumMemberElementArtifact : CodeElementArtifactBase<EnumMemberElement>
{
    public EnumMemberElementArtifact(EnumMemberElement enumMemberElement) : base(enumMemberElement) { }
    public EnumMemberElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public object? Value
    {
        get => CodeElement.Value;
        set => CodeElement.Value = value;
    }

    public bool HasExplicitValue => CodeElement.HasExplicitValue;
}
