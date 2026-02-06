using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Services
{
    /// <summary>
    /// Represents a method in a domain service interface
    /// </summary>
    public class DomainServiceMethodArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public DomainServiceMethodArtifact(string name)
            : base()
        {
            Name = name;
            ReturnType = "void";
        }

        public DomainServiceMethodArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("square-function");

        /// <summary>
        /// Method name
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
        /// Return type of the method
        /// </summary>
        public string ReturnType
        {
            get => GetValue<string>(nameof(ReturnType));
            set => SetValue(nameof(ReturnType), value);
        }

        /// <summary>
        /// Method description
        /// </summary>
        public string? Description
        {
            get => GetValue<string?>(nameof(Description));
            set => SetValue(nameof(Description), value);
        }

        /// <summary>
        /// Indicates if this method is async
        /// </summary>
        public bool IsAsync
        {
            get => GetValue<bool>(nameof(IsAsync), defaultValue: true);
            set => SetValue(nameof(IsAsync), value);
        }

        /// <summary>
        /// Parameters for this method (comma-separated, e.g., "Order order, decimal discountPercentage")
        /// </summary>
        public string? Parameters
        {
            get => GetValue<string?>(nameof(Parameters));
            set => SetValue(nameof(Parameters), value);
        }

        /// <summary>
        /// Indicates if this method performs validation
        /// </summary>
        public bool IsValidationMethod
        {
            get => GetValue<bool>(nameof(IsValidationMethod), defaultValue: false);
            set => SetValue(nameof(IsValidationMethod), value);
        }

        /// <summary>
        /// Indicates if this method performs calculations
        /// </summary>
        public bool IsCalculationMethod
        {
            get => GetValue<bool>(nameof(IsCalculationMethod), defaultValue: false);
            set => SetValue(nameof(IsCalculationMethod), value);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
