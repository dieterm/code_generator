using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Specifications
{
    /// <summary>
    /// Container artifact for domain specifications
    /// </summary>
    public class DomainSpecificationsContainerArtifact : WorkspaceArtifactBase, IEnumerable<DomainSpecificationArtifact>
    {
        public DomainSpecificationsContainerArtifact()
            : base()
        {
        }

        public DomainSpecificationsContainerArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => "Specifications";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("list-checks");

        /// <summary>
        /// Get all specifications in this container
        /// </summary>
        public IEnumerable<DomainSpecificationArtifact> GetSpecifications() =>
            Children.OfType<DomainSpecificationArtifact>();

        /// <summary>
        /// Add a specification to this container
        /// </summary>
        public void AddSpecification(DomainSpecificationArtifact specification)
        {
            AddChild(specification);
        }

        /// <summary>
        /// Remove a specification from this container
        /// </summary>
        public void RemoveSpecification(DomainSpecificationArtifact specification)
        {
            RemoveChild(specification);
        }

        public IEnumerator<DomainSpecificationArtifact> GetEnumerator()
        {
            return GetSpecifications().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
