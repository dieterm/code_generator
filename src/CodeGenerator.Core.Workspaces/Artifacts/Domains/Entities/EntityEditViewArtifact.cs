using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities
{
    /// <summary>
    /// Represents an edit view definition for an entity state.
    /// Defines which fields/controls to show when editing an entity state.
    /// </summary>
    public class EntityEditViewArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public EntityEditViewArtifact(string name)
            : base()
        {
            Name = name;
        }

        public EntityEditViewArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => $"Edit: {Name}";

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("pencil");

        /// <summary>
        /// View name
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
        /// Reference to the EntityState this edit view is for
        /// </summary>
        public string? EntityStateId
        {
            get => GetValue<string?>(nameof(EntityStateId));
            set => SetValue(nameof(EntityStateId), value);
        }

        /// <summary>
        /// Gets the referenced EntityState
        /// </summary>
        public EntityStateArtifact? EntityState
        {
            get
            {
                if (string.IsNullOrEmpty(EntityStateId)) return null;
                var entity = FindAncesterOfType<EntityArtifact>();
                return entity?.GetStates().FirstOrDefault(s => s.Id == EntityStateId);
            }
        }

        /// <summary>
        /// Get all field definitions for this edit view
        /// </summary>
        public IEnumerable<EntityEditViewFieldArtifact> GetFields() =>
            Children.OfType<EntityEditViewFieldArtifact>();

        /// <summary>
        /// Add a field definition to this edit view
        /// </summary>
        public void AddField(EntityEditViewFieldArtifact field)
        {
            AddChild(field);
        }

        /// <summary>
        /// Remove a field definition from this edit view
        /// </summary>
        public void RemoveField(EntityEditViewFieldArtifact field)
        {
            RemoveChild(field);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
