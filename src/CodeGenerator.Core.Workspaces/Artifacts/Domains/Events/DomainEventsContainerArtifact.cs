using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Events
{
    /// <summary>
    /// Container artifact for domain events
    /// </summary>
    public class DomainEventsContainerArtifact : WorkspaceArtifactBase, IEnumerable<DomainEventArtifact>
    {
        public DomainEventsContainerArtifact()
            : base()
        {
        }

        public DomainEventsContainerArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => "Events";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("zap");

        /// <summary>
        /// Get all domain events in this container
        /// </summary>
        public IEnumerable<DomainEventArtifact> GetDomainEvents() =>
            Children.OfType<DomainEventArtifact>();

        /// <summary>
        /// Add a domain event to this container
        /// </summary>
        public void AddDomainEvent(DomainEventArtifact domainEvent)
        {
            AddChild(domainEvent);
        }

        /// <summary>
        /// Remove a domain event from this container
        /// </summary>
        public void RemoveDomainEvent(DomainEventArtifact domainEvent)
        {
            RemoveChild(domainEvent);
        }

        public IEnumerator<DomainEventArtifact> GetEnumerator()
        {
            return GetDomainEvents().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
