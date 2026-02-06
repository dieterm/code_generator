using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Repositories
{
    /// <summary>
    /// Represents a custom method in a repository interface
    /// </summary>
    public class DomainRepositoryMethodArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public DomainRepositoryMethodArtifact(string name)
            : base()
        {
            Name = name;
            ReturnType = "void";
        }

        public DomainRepositoryMethodArtifact(ArtifactState state)
            : base(state)
        {
        }
        
        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("function");

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
        /// Parameters for this method (comma-separated, e.g., "string name, int age")
        /// </summary>
        public string? Parameters
        {
            get => GetValue<string?>(nameof(Parameters));
            set => SetValue(nameof(Parameters), value);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
