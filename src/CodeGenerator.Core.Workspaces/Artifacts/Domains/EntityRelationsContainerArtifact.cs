using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    /// <summary>
    /// Container artifact for entity relations
    /// </summary>
    public class EntityRelationsContainerArtifact : Artifact, IEnumerable<EntityRelationArtifact>
    {
        public EntityRelationsContainerArtifact()
            : base()
        {
        }

        public EntityRelationsContainerArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => "Relations";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("link");

        /// <summary>
        /// Get all relations in this container
        /// </summary>
        public IEnumerable<EntityRelationArtifact> GetRelations() =>
            Children.OfType<EntityRelationArtifact>();

        /// <summary>
        /// Add a relation to this container
        /// </summary>
        public void AddRelation(EntityRelationArtifact relation)
        {
            AddChild(relation);
        }

        /// <summary>
        /// Remove a relation from this container
        /// </summary>
        public void RemoveRelation(EntityRelationArtifact relation)
        {
            RemoveChild(relation);
        }

        public IEnumerator<EntityRelationArtifact> GetEnumerator()
        {
            return GetRelations().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
