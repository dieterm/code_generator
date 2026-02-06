using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Factories
{
    /// <summary>
    /// Container artifact for domain factories
    /// </summary>
    public class DomainFactoriesContainerArtifact : WorkspaceArtifactBase, IEnumerable<DomainFactoryArtifact>
    {
        public DomainFactoriesContainerArtifact()
            : base()
        {
        }

        public DomainFactoriesContainerArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => "Factories";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("factory");

        /// <summary>
        /// Get all factories in this container
        /// </summary>
        public IEnumerable<DomainFactoryArtifact> GetFactories() =>
            Children.OfType<DomainFactoryArtifact>();

        /// <summary>
        /// Add a factory to this container
        /// </summary>
        public void AddFactory(DomainFactoryArtifact factory)
        {
            AddChild(factory);
        }

        /// <summary>
        /// Remove a factory from this container
        /// </summary>
        public void RemoveFactory(DomainFactoryArtifact factory)
        {
            RemoveChild(factory);
        }

        public IEnumerator<DomainFactoryArtifact> GetEnumerator()
        {
            return GetFactories().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
