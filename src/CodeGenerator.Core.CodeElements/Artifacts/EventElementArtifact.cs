using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class EventElementArtifact : CodeElementArtifactBase<EventElement>
{
    public EventElementArtifact(EventElement eventElement) : base(eventElement) { }
    public EventElementArtifact(ArtifactState artifactState, List<string> errors) : base(artifactState, errors) { }

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

    public bool IsFieldLike
    {
        get { return CodeElement.IsFieldLike; }
        set
        {
            if (CodeElement.IsFieldLike != value)
            {
                CodeElement.IsFieldLike = value;
                RaisePropertyChangedEvent(nameof(IsFieldLike));
            }
        }
    }
}
