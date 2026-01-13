using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    /// <summary>
    /// Represents a workspace - the root container for all datasources and configuration
    /// </summary>
    public class WorkspaceArtifact : Artifact, IEditableTreeNode
    {
        public WorkspaceArtifact(string name = "Workspace")
        {
            Name = name;
            RootNamespace = "MyCompany.MyProduct";
            DefaultOutputDirectory = string.Empty;
            DefaultTargetFramework = "net8.0";
            DefaultLanguage = "C#";
            WorkspaceFilePath = string.Empty;
        }

        public WorkspaceArtifact(ArtifactState state)
            : base(state)
        {
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("book-text");

        /// <summary>
        /// Display name of the workspace
        /// </summary>
        public string Name
        {
            get => GetValue<string>(nameof(Name));
            set { 
                if(SetValue(nameof(Name), value))
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Root namespace for generated code
        /// </summary>
        public string RootNamespace
        {
            get => GetValue<string>(nameof(RootNamespace));
            set { SetValue(nameof(RootNamespace), value); }
        }

        /// <summary>
        /// Default output directory for generated code
        /// </summary>
        public string DefaultOutputDirectory
        {
            get => GetValue<string>(nameof(DefaultOutputDirectory));
            set { SetValue(nameof(DefaultOutputDirectory), value); }
        }

        /// <summary>
        /// Default target framework for new projects
        /// </summary>
        public string DefaultTargetFramework
        {
            get => GetValue<string>(nameof(DefaultTargetFramework));
            set { SetValue(nameof(DefaultTargetFramework), value); }
        }

        /// <summary>
        /// Default programming language
        /// </summary>
        public string DefaultLanguage
        {
            get => GetValue<string>(nameof(DefaultLanguage));
            set { SetValue(nameof(DefaultLanguage), value); }
        }

        /// <summary>
        /// Path to the .codegenerator file
        /// </summary>
        public string WorkspaceFilePath
        {
            get => GetValue<string>(nameof(WorkspaceFilePath));
            set { if (SetValue(nameof(WorkspaceFilePath), value))
                    RaisePropertyChangedEvent(nameof(WorkspaceDirectory));
            }
        }

        /// <summary>
        /// Directory containing the workspace
        /// </summary>
        public string WorkspaceDirectory => 
            string.IsNullOrEmpty(WorkspaceFilePath) 
                ? string.Empty 
                : Path.GetDirectoryName(WorkspaceFilePath) ?? string.Empty;

        /// <summary>
        /// Gets the datasources container
        /// </summary>
        public DatasourcesContainerArtifact Datasources { 
            get { 
                return Children.OfType<DatasourcesContainerArtifact>().First();
            } 
        }
    }
}
