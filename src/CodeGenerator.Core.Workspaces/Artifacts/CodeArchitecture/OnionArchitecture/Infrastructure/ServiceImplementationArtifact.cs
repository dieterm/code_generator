using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    /// <summary>
    /// Represents a ServicesImplementation
    /// </summary>
    public class ServiceImplementationArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public ServiceImplementationArtifact(string name)
            : base()
        {
            Name = name;
        }

        public ServiceImplementationArtifact(ArtifactState state, List<string> errors)
            : base(state, errors)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("hand-helping");

        /// <summary>
        /// ServicesImplementation name
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
        /// Description of the ServicesImplementation
        /// </summary>
        public string? Description
        {
            get => GetValue<string?>(nameof(Description));
            set => SetValue(nameof(Description), value);
        }

        public bool CanBeginEdit() => true;

        public bool Validating(string newName) => !string.IsNullOrWhiteSpace(newName);

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
