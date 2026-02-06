using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Specifications
{
    /// <summary>
    /// Represents a specification in the domain layer (DDD Specification Pattern)
    /// </summary>
    public class DomainSpecificationArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public DomainSpecificationArtifact(string name)
            : base()
        {
            Name = name;
        }

        public DomainSpecificationArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("square-check-big");

        /// <summary>
        /// Specification name
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
        /// Specification description for documentation purposes
        /// </summary>
        public string? Description
        {
            get => GetValue<string?>(nameof(Description));
            set => SetValue(nameof(Description), value);
        }

        /// <summary>
        /// Reference to the entity this specification applies to (by ID)
        /// </summary>
        public string? EntityId
        {
            get => GetValue<string?>(nameof(EntityId));
            set => SetValue(nameof(EntityId), value);
        }

        /// <summary>
        /// Indicates if this is a composite specification (combines multiple specifications)
        /// </summary>
        public bool IsComposite
        {
            get => GetValue<bool>(nameof(IsComposite), defaultValue: false);
            set => SetValue(nameof(IsComposite), value);
        }

        /// <summary>
        /// Expression or criteria for the specification (textual representation)
        /// </summary>
        public string? Criteria
        {
            get => GetValue<string?>(nameof(Criteria));
            set => SetValue(nameof(Criteria), value);
        }

        /// <summary>
        /// Category of the specification (e.g., "Business Rule", "Query", "Validation")
        /// </summary>
        public string? Category
        {
            get => GetValue<string?>(nameof(Category));
            set => SetValue(nameof(Category), value);
        }

        /// <summary>
        /// Indicates if this specification is reusable across multiple contexts
        /// </summary>
        public bool IsReusable
        {
            get => GetValue<bool>(nameof(IsReusable), defaultValue: true);
            set => SetValue(nameof(IsReusable), value);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
