using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Services
{
    /// <summary>
    /// Represents a domain service interface in the domain layer (DDD)
    /// </summary>
    public class DomainServiceArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public DomainServiceArtifact(string name)
            : base()
        {
            Name = name;
        }

        public DomainServiceArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("hand-helping");

        /// <summary>
        /// Domain service interface name
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
        /// Description of the domain service
        /// </summary>
        public string? Description
        {
            get => GetValue<string?>(nameof(Description));
            set => SetValue(nameof(Description), value);
        }

        /// <summary>
        /// Indicates if this service is stateless
        /// </summary>
        public bool IsStateless
        {
            get => GetValue<bool>(nameof(IsStateless), defaultValue: true);
            set => SetValue(nameof(IsStateless), value);
        }

        /// <summary>
        /// Category or purpose of the domain service (e.g., "Validation", "Calculation", "Integration")
        /// </summary>
        public string? Category
        {
            get => GetValue<string?>(nameof(Category));
            set => SetValue(nameof(Category), value);
        }

        /// <summary>
        /// Indicates if this service supports async operations
        /// </summary>
        public bool SupportsAsync
        {
            get => GetValue<bool>(nameof(SupportsAsync), defaultValue: true);
            set => SetValue(nameof(SupportsAsync), value);
        }

        /// <summary>
        /// Get all methods for this domain service
        /// </summary>
        public IEnumerable<DomainServiceMethodArtifact> Methods => Children.OfType<DomainServiceMethodArtifact>();

        /// <summary>
        /// Add a method to this domain service
        /// </summary>
        public void AddMethod(DomainServiceMethodArtifact method)
        {
            AddChild(method);
        }

        /// <summary>
        /// Remove a method from this domain service
        /// </summary>
        public void RemoveMethod(DomainServiceMethodArtifact method)
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
