using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Repositories
{
    /// <summary>
    /// Represents a repository interface in the domain layer (DDD)
    /// </summary>
    public class DomainRepositoryArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public DomainRepositoryArtifact(string name)
            : base()
        {
            Name = name;
        }

        public DomainRepositoryArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("database");

        /// <summary>
        /// Repository interface name
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
        /// Description of the repository
        /// </summary>
        public string? Description
        {
            get => GetValue<string?>(nameof(Description));
            set => SetValue(nameof(Description), value);
        }

        /// <summary>
        /// Reference to the aggregate root entity this repository manages (by ID)
        /// </summary>
        public string? AggregateRootEntityId
        {
            get => GetValue<string?>(nameof(AggregateRootEntityId));
            set => SetValue(nameof(AggregateRootEntityId), value);
        }

        /// <summary>
        /// Indicates if this repository supports async operations
        /// </summary>
        public bool SupportsAsync
        {
            get => GetValue<bool>(nameof(SupportsAsync), defaultValue: true);
            set => SetValue(nameof(SupportsAsync), value);
        }

        /// <summary>
        /// Indicates if this repository includes GetById method
        /// </summary>
        public bool IncludeGetById
        {
            get => GetValue<bool>(nameof(IncludeGetById), defaultValue: true);
            set => SetValue(nameof(IncludeGetById), value);
        }

        /// <summary>
        /// Indicates if this repository includes GetAll method
        /// </summary>
        public bool IncludeGetAll
        {
            get => GetValue<bool>(nameof(IncludeGetAll), defaultValue: true);
            set => SetValue(nameof(IncludeGetAll), value);
        }

        /// <summary>
        /// Indicates if this repository includes Add method
        /// </summary>
        public bool IncludeAdd
        {
            get => GetValue<bool>(nameof(IncludeAdd), defaultValue: true);
            set => SetValue(nameof(IncludeAdd), value);
        }

        /// <summary>
        /// Indicates if this repository includes Update method
        /// </summary>
        public bool IncludeUpdate
        {
            get => GetValue<bool>(nameof(IncludeUpdate), defaultValue: true);
            set => SetValue(nameof(IncludeUpdate), value);
        }

        /// <summary>
        /// Indicates if this repository includes Delete method
        /// </summary>
        public bool IncludeDelete
        {
            get => GetValue<bool>(nameof(IncludeDelete), defaultValue: true);
            set => SetValue(nameof(IncludeDelete), value);
        }

        /// <summary>
        /// Indicates if this repository supports specifications
        /// </summary>
        public bool SupportsSpecifications
        {
            get => GetValue<bool>(nameof(SupportsSpecifications), defaultValue: false);
            set => SetValue(nameof(SupportsSpecifications), value);
        }

        /// <summary>
        /// Indicates if this repository supports pagination
        /// </summary>
        public bool SupportsPagination
        {
            get => GetValue<bool>(nameof(SupportsPagination), defaultValue: false);
            set => SetValue(nameof(SupportsPagination), value);
        }

        /// <summary>
        /// Get all custom methods for this repository
        /// </summary>
        public IEnumerable<DomainRepositoryMethodArtifact> Methods => Children.OfType<DomainRepositoryMethodArtifact>();

        /// <summary>
        /// Add a custom method to this repository
        /// </summary>
        public void AddMethod(DomainRepositoryMethodArtifact method)
        {
            AddChild(method);
        }

        /// <summary>
        /// Remove a custom method from this repository
        /// </summary>
        public void RemoveMethod(DomainRepositoryMethodArtifact method)
        {
            RemoveChild(method);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
