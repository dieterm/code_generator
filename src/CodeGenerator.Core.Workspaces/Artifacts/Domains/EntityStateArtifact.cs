using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    /// <summary>
    /// Represents a state of an entity
    /// </summary>
    public class EntityStateArtifact : Artifact, IEditableTreeNode
    {
        public EntityStateArtifact(string name)
            : base()
        {
            Name = name;
        }

        public EntityStateArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("square-arrow-left");

        /// <summary>
        /// State name
        /// </summary>
        public string Name
        {
            get => GetValue<string>(nameof(Name));
            set
            {
                if (SetValue(nameof(Name), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }



        /// <summary>
        /// Get all properties for this state
        /// </summary>
        public new IEnumerable<PropertyArtifact> Properties { get { return Children.OfType<PropertyArtifact>(); } }

        /// <summary>
        /// Add a property to this state
        /// </summary>
        public void AddProperty(PropertyArtifact property)
        {
            AddChild(property);
        }

        /// <summary>
        /// Remove a property from this state
        /// </summary>
        public void RemoveProperty(PropertyArtifact property)
        {
            RemoveChild(property);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
