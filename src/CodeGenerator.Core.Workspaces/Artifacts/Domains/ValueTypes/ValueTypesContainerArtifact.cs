using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;
using System.Collections;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes
{
    /// <summary>
    /// Container artifact for value types
    /// </summary>
    public class ValueTypesContainerArtifact : WorkspaceArtifactBase, IEnumerable<ValueTypeArtifact>
    {
        public ValueTypesContainerArtifact()
            : base()
        {
        }

        public ValueTypesContainerArtifact(ArtifactState state, List<string> errors)
            : base(state, errors)
        {
        }

        public override string TreeNodeText => "Value Types";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("blocks");

        /// <summary>
        /// Get all value types in this container
        /// </summary>
        public IEnumerable<ValueTypeArtifact> GetValueTypes() =>
            Children.OfType<ValueTypeArtifact>();

        /// <summary>
        /// Add a value type to this container
        /// </summary>
        public ValueTypeArtifact AddValueType(ValueTypeArtifact valueType)
        {
            AddChild(valueType);
            return valueType;
        }

        /// <summary>
        /// Remove a value type from this container
        /// </summary>
        public void RemoveValueType(ValueTypeArtifact valueType)
        {
            RemoveChild(valueType);
        }

        public IEnumerator<ValueTypeArtifact> GetEnumerator()
        {
            return GetValueTypes().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected override WorkspaceArtifactContext? GetOwnContext()
        {
            return new WorkspaceArtifactContext { 
                Namespace = $"{(Parent as DomainArtifact).Namespace}.ValueTypes"
            };
        }
    }
}
