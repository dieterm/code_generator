using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Core.Workspaces.ViewModels;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Views.TreeNode;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    /// <summary>
    /// Represents a workspace - the root container for all datasources and configuration
    /// </summary>
    public partial class WorkspaceArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public const string CONTEXT_PARAMETER_WORKSPACE_ROOT_NAMESPACE = "WorkspaceRootNamespace";
        public const string CONTEXT_PARAMETER_LANGUAGE = "Language";

        public WorkspaceArtifact(string name = "Workspace")
        {
            Name = name;
            
            RootNamespace = WorkspaceSettings.Instance.RootNamespace;
            OutputDirectory = WorkspaceSettings.Instance.DefaultOutputDirectory;
            DefaultTargetFramework = WorkspaceSettings.Instance.DefaultTargetFramework;
            DefaultLanguage = WorkspaceSettings.Instance.DefaultLanguage;
            DependencyInjectionFrameworkId = WorkspaceSettings.Instance.DefaultDependencyInjectionFrameworkId;
            CodeArchitectureId = WorkspaceSettings.Instance.DefaultCodeArchitectureId;

            PublishArtifactConstructionEvent();
        }

        public WorkspaceArtifact(ArtifactState state)
            : base(state)
        {

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
            set
            {
                if (SetValue(nameof(Name), value)) { 
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                    RaiseContextChanged();
                }
            }
        }

        /// <summary>
        /// Root namespace for generated code
        /// </summary>
        public string RootNamespace
        {
            get => GetValue<string>(nameof(RootNamespace));
            set { 
                if (SetValue(nameof(RootNamespace), value))
                {
                    RaiseContextChanged();
                }
            }
        }

        /// <summary>
        /// Default output directory for generated code
        /// </summary>
        public string OutputDirectory
        {
            get => GetValue<string>(nameof(OutputDirectory));
            set {
                if (SetValue(nameof(OutputDirectory), value))
                {
                    RaiseContextChanged();
                }
            }
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

        public string? CodeArchitectureId
        {
            get => GetValue<string?>(nameof(CodeArchitectureId));
            set { SetValue(nameof(CodeArchitectureId), value); }
        }

        public ICodeArchitecture? CodeArchitecture
        {
            get
            {
                if (string.IsNullOrEmpty(CodeArchitectureId))
                    return null;
                var architectureManager = ServiceProviderHolder.GetRequiredService<CodeArchitectureManager>();
                //var allArchitectures = architectureManager.GetAllArchitectures();
                return architectureManager.GetById(CodeArchitectureId);
            }
        }

        /// <summary>
        /// The ID of the dependency injection framework to use for code generation
        /// </summary>
        public string? DependencyInjectionFrameworkId
        {
            get => GetValue<string?>(nameof(DependencyInjectionFrameworkId));
            set { SetValue(nameof(DependencyInjectionFrameworkId), value); }
        }

        /// <summary>
        /// Get the dependency injection framework instance for this workspace
        /// </summary>
        public DependancyInjectionFramework? DependencyInjectionFramework
        {
            get
            {
                if (string.IsNullOrEmpty(DependencyInjectionFrameworkId))
                    return null;
                var frameworkManager = ServiceProviderHolder.GetRequiredService<DependancyInjectionFrameworkManager>();
                return frameworkManager.GetFrameworkById(DependencyInjectionFrameworkId);
            }
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
        /// Information about the workspace/project, used in documentation and README generation
        /// </summary>
        public string Documentation {
            get { return GetValue<string>(nameof(Documentation)); }
            set { SetValue(nameof(Documentation), value); }
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

        //public ScopeArtifact? FindScopeOrDefault(string scopeName)
        //{
        //    return Scopes.FindScope(scopeName, false);
        //}

        public ScopeArtifact? FindScope(string scopeName, bool exceptionIfNotFound = true)
        {
            return Scopes.FindScope(scopeName, exceptionIfNotFound);
        }

        public DomainArtifact? FindDomain(string scopeName, string domainName, bool exceptionIfNotFound = true)
        {
            var scope = FindScope(scopeName);
            return scope?.FindDomain(domainName, exceptionIfNotFound);
        }

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

        public override WorkspaceArtifact? Workspace => this;

        

        protected override WorkspaceArtifactContext GetOwnContext()
        {
            var namespaceParameters = new Dictionary<string, string>();
            namespaceParameters.Add(CONTEXT_PARAMETER_WORKSPACE_ROOT_NAMESPACE, RootNamespace);
            namespaceParameters.Add(CONTEXT_PARAMETER_LANGUAGE, DefaultLanguage);
            return new WorkspaceArtifactContext
            {
                NamespaceParameters = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(namespaceParameters),
                Namespace = RootNamespace,
                OutputPath = OutputDirectory                
            };
        }

        public IEnumerable<ScopeArtifact> GetAllScopes(bool onlyUserScopes, bool includeSubScopes)
        {
            IEnumerable<ScopeArtifact> scopes = Scopes;
            if (includeSubScopes) 
                scopes = FindDescendants<ScopeArtifact>();
            
            foreach (var scope in scopes)
            {
                if(scope.IsDefaultScope() && onlyUserScopes)
                    continue;
                yield return scope;
                
            }
        }
    }
}
