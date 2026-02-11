using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class EventsContainerArtifact : CodeElementArtifactBase, IEnumerable<EventElementArtifact>
{
    private readonly List<EventElement> _events;

    public EventsContainerArtifact(List<EventElement> events) : base()
    {
        _events = events;
        foreach (var evt in events)
            AddChild(new EventElementArtifact(evt));
    }

    public EventsContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Events";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public void AddNewEvent()
    {
        var evt = new EventElement("NewEvent", new TypeReference("EventHandler"));
        _events.Add(evt);
        AddChild(new EventElementArtifact(evt));
    }

    public void RemoveEvent(EventElementArtifact artifact)
    {
        _events.Remove(artifact.CodeElement);
        RemoveChild(artifact);
    }

    public IEnumerator<EventElementArtifact> GetEnumerator() => Children.OfType<EventElementArtifact>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
