using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Services
{
    /// <summary>
    /// Container artifact for domain services
    /// </summary>
    public class DomainServicesContainerArtifact : WorkspaceArtifactBase, IEnumerable<DomainServiceArtifact>
    {
        public DomainServicesContainerArtifact()
            : base()
        {
        }

        public DomainServicesContainerArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => "Domain Services";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("hand-helping");

        /// <summary>
        /// Get all domain services in this container
        /// </summary>
        public IEnumerable<DomainServiceArtifact> GetDomainServices() =>
            Children.OfType<DomainServiceArtifact>();

        /// <summary>
        /// Add a domain service to this container
        /// </summary>
        public void AddDomainService(DomainServiceArtifact service)
        {
            AddChild(service);
        }

        /// <summary>
        /// Remove a domain service from this container
        /// </summary>
        public void RemoveDomainService(DomainServiceArtifact service)
        {
            RemoveChild(service);
        }

        public IEnumerator<DomainServiceArtifact> GetEnumerator()
        {
            return GetDomainServices().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
