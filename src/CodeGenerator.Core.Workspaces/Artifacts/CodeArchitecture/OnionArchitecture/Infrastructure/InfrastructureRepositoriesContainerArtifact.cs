using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    /// <summary>
    /// Container artifact for RepositoryImplementation
    /// </summary>
    public class InfrastructureRepositoriesContainerArtifact : WorkspaceArtifactBase, IEnumerable<RepositoryImplementationArtifact>
    {
        public InfrastructureRepositoriesContainerArtifact()
            : base()
        {
        }

        public InfrastructureRepositoriesContainerArtifact(ArtifactState state, List<string> errors)
            : base(state, errors)
        {
        }

        public override string TreeNodeText => "Repositories";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("database");

        /// <summary>
        /// Get all InfrastructureRepositories in this container
        /// </summary>
        public IEnumerable<RepositoryImplementationArtifact> GetInfrastructureRepositories() =>
            Children.OfType<RepositoryImplementationArtifact>();

        /// <summary>
        /// Add a RepositoryImplementation to this container
        /// </summary>
        public void AddRepositoryImplementation(RepositoryImplementationArtifact state)
        {
            AddChild(state);
        }

        /// <summary>
        /// Remove a RepositoryImplementation from this container
        /// </summary>
        public void RemoveRepositoryImplementation(RepositoryImplementationArtifact state)
        {
            RemoveChild(state);
        }

        public IEnumerator<RepositoryImplementationArtifact> GetEnumerator()
        {
            return GetInfrastructureRepositories().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
