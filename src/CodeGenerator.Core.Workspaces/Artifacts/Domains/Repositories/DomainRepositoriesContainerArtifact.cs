using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Repositories
{
    /// <summary>
    /// Container artifact for domain repositories
    /// </summary>
    public class DomainRepositoriesContainerArtifact : WorkspaceArtifactBase, IEnumerable<DomainRepositoryArtifact>
    {
        public DomainRepositoriesContainerArtifact()
            : base()
        {
        }

        public DomainRepositoriesContainerArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => "Repositories";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("database");

        /// <summary>
        /// Get all repositories in this container
        /// </summary>
        public IEnumerable<DomainRepositoryArtifact> GetRepositories() =>
            Children.OfType<DomainRepositoryArtifact>();

        /// <summary>
        /// Add a repository to this container
        /// </summary>
        public void AddRepository(DomainRepositoryArtifact repository)
        {
            AddChild(repository);
        }

        /// <summary>
        /// Remove a repository from this container
        /// </summary>
        public void RemoveRepository(DomainRepositoryArtifact repository)
        {
            RemoveChild(repository);
        }

        public IEnumerator<DomainRepositoryArtifact> GetEnumerator()
        {
            return GetRepositories().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
