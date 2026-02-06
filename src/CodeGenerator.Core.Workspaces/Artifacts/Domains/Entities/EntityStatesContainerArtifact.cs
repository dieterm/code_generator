using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities
{
    /// <summary>
    /// Container artifact for entity states
    /// </summary>
    public class EntityStatesContainerArtifact : WorkspaceArtifactBase, IEnumerable<EntityStateArtifact>
    {
        public EntityStatesContainerArtifact()
            : base()
        {
        }

        public EntityStatesContainerArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => "States";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("between-horizontal-end");

        /// <summary>
        /// Get all states in this container
        /// </summary>
        public IEnumerable<EntityStateArtifact> GetStates() =>
            Children.OfType<EntityStateArtifact>();

        /// <summary>
        /// Add a state to this container
        /// </summary>
        public void AddState(EntityStateArtifact state)
        {
            AddChild(state);
        }

        /// <summary>
        /// Remove a state from this container
        /// </summary>
        public void RemoveState(EntityStateArtifact state)
        {
            RemoveChild(state);
        }

        public IEnumerator<EntityStateArtifact> GetEnumerator()
        {
            return GetStates().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
