using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Factories
{
    /// <summary>
    /// Represents a factory in the domain layer (DDD Factory Pattern)
    /// </summary>
    public class DomainFactoryArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public DomainFactoryArtifact(string name)
            : base()
        {
            Name = name;
        }

        public DomainFactoryArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("factory");

        /// <summary>
        /// Factory name
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
        /// Factory description for documentation purposes
        /// </summary>
        public string? Description
        {
            get => GetValue<string?>(nameof(Description));
            set => SetValue(nameof(Description), value);
        }

        /// <summary>
        /// Reference to the entity this factory creates (by ID)
        /// </summary>
        public string? EntityId
        {
            get => GetValue<string?>(nameof(EntityId));
            set => SetValue(nameof(EntityId), value);
        }

        /// <summary>
        /// Indicates if this factory creates aggregates
        /// </summary>
        public bool CreatesAggregates
        {
            get => GetValue<bool>(nameof(CreatesAggregates), defaultValue: false);
            set => SetValue(nameof(CreatesAggregates), value);
        }

        /// <summary>
        /// Indicates if this factory is stateless
        /// </summary>
        public bool IsStateless
        {
            get => GetValue<bool>(nameof(IsStateless), defaultValue: true);
            set => SetValue(nameof(IsStateless), value);
        }

        /// <summary>
        /// Category of the factory (e.g., "Entity Factory", "Value Object Factory", "Aggregate Factory")
        /// </summary>
        public string? Category
        {
            get => GetValue<string?>(nameof(Category));
            set => SetValue(nameof(Category), value);
        }

        /// <summary>
        /// Indicates if this factory has dependencies
        /// </summary>
        public bool HasDependencies
        {
            get => GetValue<bool>(nameof(HasDependencies), defaultValue: false);
            set => SetValue(nameof(HasDependencies), value);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
