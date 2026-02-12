using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class EnumMemberElementArtifact : CodeElementArtifactBase<EnumMemberElement>
{
    public EnumMemberElementArtifact(EnumMemberElement enumMemberElement) : base(enumMemberElement) { }
    public EnumMemberElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public object? Value
    {
        get { return CodeElement.Value; }
        set
        {
            if (!Equals(CodeElement.Value, value))
            {
                CodeElement.Value = value;
                RaisePropertyChangedEvent(nameof(Value));
            }
        }
    }

    public bool HasExplicitValue => CodeElement.HasExplicitValue;
}
