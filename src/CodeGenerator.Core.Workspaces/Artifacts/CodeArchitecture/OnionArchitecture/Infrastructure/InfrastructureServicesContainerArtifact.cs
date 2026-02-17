using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    /// <summary>
    /// Container artifact for ServicesImplementation
    /// </summary>
    public class InfrastructureServicesContainerArtifact : WorkspaceArtifactBase, IEnumerable<ServiceImplementationArtifact>
    {
        public InfrastructureServicesContainerArtifact()
            : base()
        {
        }

        public InfrastructureServicesContainerArtifact(ArtifactState state, List<string> errors)
            : base(state, errors)
        {
        }

        public override string TreeNodeText => "Services";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("hand-helping");

        /// <summary>
        /// Get all InfrastructureServices in this container
        /// </summary>
        public IEnumerable<ServiceImplementationArtifact> GetInfrastructureServices() =>
            Children.OfType<ServiceImplementationArtifact>();

        /// <summary>
        /// Add a ServicesImplementation to this container
        /// </summary>
        public void AddServicesImplementation(ServiceImplementationArtifact state)
        {
            AddChild(state);
        }

        /// <summary>
        /// Remove a ServicesImplementation from this container
        /// </summary>
        public void RemoveServicesImplementation(ServiceImplementationArtifact state)
        {
            RemoveChild(state);
        }

        public IEnumerator<ServiceImplementationArtifact> GetEnumerator()
        {
            return GetInfrastructureServices().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
