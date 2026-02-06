using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes
{
    /// <summary>
    /// Represents a value type in the domain
    /// </summary>
    public class ValueTypeArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public ValueTypeArtifact(string name)
            : base()
        {
            Name = name;
        }

        public ValueTypeArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("square-arrow-left");

        /// <summary>
        /// Value type name
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
        /// Description of the value type
        /// </summary>
        public string? Description
        {
            get => GetValue<string?>(nameof(Description));
            set => SetValue(nameof(Description), value);
        }

        /// <summary>
        /// Get all properties for this value type
        /// </summary>
        public new IEnumerable<PropertyArtifact> Properties => Children.OfType<PropertyArtifact>();

        /// <summary>
        /// Add a property to this value type
        /// </summary>
        public void AddProperty(PropertyArtifact property)
        {
            AddChild(property);
        }

        /// <summary>
        /// Remove a property from this value type
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
