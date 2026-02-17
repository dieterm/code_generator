using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    /// <summary>
    /// Represents a RepositoryImplementation
    /// </summary>
    public class RepositoryImplementationArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public RepositoryImplementationArtifact(string name)
            : base()
        {
            Name = name;
        }

        public RepositoryImplementationArtifact(ArtifactState state, List<string> errors)
            : base(state, errors)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("database");

        /// <summary>
        /// RepositoryImplementation name
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
        /// Description of the RepositoryImplementation
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
