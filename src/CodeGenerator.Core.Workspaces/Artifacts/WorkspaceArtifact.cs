using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    /// <summary>
    /// Represents a workspace - the root container for all datasources and configuration
    /// </summary>
    public partial class WorkspaceArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public const string ProjectNamePattern_LanguageParameter = "Language";
        public const string ProjectNamePattern_WorkspaceNamespaceParameter = "WorkspaceNamespace";

        public WorkspaceArtifact(string name = "Workspace")
        {
            Name = name;
            
            RootNamespace = WorkspaceSettings.Instance.RootNamespace;
            DefaultOutputDirectory = WorkspaceSettings.Instance.DefaultOutputDirectory;
            DefaultTargetFramework = WorkspaceSettings.Instance.DefaultTargetFramework;
            DefaultLanguage = WorkspaceSettings.Instance.DefaultLanguage;
            //WorkspaceFilePath = string.Empty;

            EnsureChildArtifactExists<DatasourcesContainerArtifact>();
            EnsureChildArtifactExists<ScopesContainerArtifact>();

            PublishArtifactConstructionEvent();
        }

        public WorkspaceArtifact(ArtifactState state)
            : base(state)
        {
            EnsureChildArtifactExists<DatasourcesContainerArtifact>();
            EnsureChildArtifactExists<ScopesContainerArtifact>();

            PublishArtifactConstructionEvent();
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
        public DatasourcesContainerArtifact Datasources { get { return this.EnsureChildArtifactExists<DatasourcesContainerArtifact>(); } }

        /// <summary>
        /// Gets the scopes container
        /// </summary>
        public ScopesContainerArtifact Scopes { get { return this.EnsureChildArtifactExists<ScopesContainerArtifact>(); } }

        public bool CanBeginEdit()
        {
            return true;
        }

        public bool Validating(string newName)
        {
            return !string.IsNullOrWhiteSpace(newName);
        }

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }
    }
}
