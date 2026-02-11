using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class EventElementArtifact : CodeElementArtifactBase<EventElement>
{
    public EventElementArtifact(EventElement eventElement) : base(eventElement) { }
    public EventElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public string TypeName
    {
        get => CodeElement.Type.TypeName;
        set => CodeElement.Type.TypeName = value;
    }

    public bool IsFieldLike
    {
        get => CodeElement.IsFieldLike;
        set => CodeElement.IsFieldLike = value;
    }
}
