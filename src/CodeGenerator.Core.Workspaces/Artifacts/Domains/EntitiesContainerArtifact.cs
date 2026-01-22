using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    /// <summary>
    /// Container artifact for entities within a domain
    /// </summary>
    public class EntitiesContainerArtifact : Artifact, IEnumerable<EntityArtifact>
    {
        public EntitiesContainerArtifact()
            : base()
        {
        }

        public EntitiesContainerArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => "Entities";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("boxes");

        /// <summary>
        /// Get all entities in this container
        /// </summary>
        public IEnumerable<EntityArtifact> GetEntities() =>
            Children.OfType<EntityArtifact>();

        /// <summary>
        /// Add an entity to this container
        /// </summary>
        public void AddEntity(EntityArtifact entity)
        {
            AddChild(entity);
        }

        /// <summary>
        /// Remove an entity from this container
        /// </summary>
        public void RemoveEntity(EntityArtifact entity)
        {
            RemoveChild(entity);
        }

        public IEnumerator<EntityArtifact> GetEnumerator()
        {
            return GetEntities().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
