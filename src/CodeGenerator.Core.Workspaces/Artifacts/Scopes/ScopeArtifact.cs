using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Shared.Views.TreeNode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGenerator.Core.Workspaces.Artifacts.Scopes
{
    public class ScopeArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public const string DEFAULT_SCOPE_SHARED = "Shared";
        public static Color  DEFAULT_SCOPE_SHARED_COLOR = Color.Blue;
        public const string DEFAULT_SCOPE_APPLICATION = "Application";
        public static Color  DEFAULT_SCOPE_APPLICATION_COLOR = Color.Green;

        public ScopeArtifact(string name)
            : base()
        {
            Name = name;
            
            AddChild(new DomainsContainerArtifact());
            AddChild(new InfrastructuresContainerArtifact());
            AddChild(new ApplicationsContainerArtifact());
            AddChild(new PresentationsContainerArtifact());
            AddChild(new SubScopesContainerArtifact());
            
            PublishArtifactConstructionEvent();
        }

        public ScopeArtifact(ArtifactState state)
            : base(state) 
        {
            EnsureChildArtifactExists<DomainsContainerArtifact>();
            EnsureChildArtifactExists<InfrastructuresContainerArtifact>();
            EnsureChildArtifactExists<ApplicationsContainerArtifact>();
            EnsureChildArtifactExists<PresentationsContainerArtifact>();
            EnsureChildArtifactExists<SubScopesContainerArtifact>();

            PublishArtifactConstructionEvent();
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("loader-pinwheel");
        public override Color? TreeNodeTextColor 
        { 
            get { return Name == DEFAULT_SCOPE_SHARED ? DEFAULT_SCOPE_SHARED_COLOR : (Name == DEFAULT_SCOPE_APPLICATION ? DEFAULT_SCOPE_APPLICATION_COLOR : (Color?)null); }
        }
        /// <summary>
        /// Display name of the scope
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(nameof(Name)); }
            set
            {
                SetValue<string>(nameof(Name), value);
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Default namespace of the scope
        /// </summary>
        public string NamespacePattern
        {
            get { return GetValue<string>(nameof(NamespacePattern)); }
            set { SetValue(nameof(NamespacePattern), value); }
        }

        public DomainsContainerArtifact Domains { get { return Children.OfType<DomainsContainerArtifact>().FirstOrDefault()!; } }
        public InfrastructuresContainerArtifact Infrastructure { get { return Children.OfType<InfrastructuresContainerArtifact>().FirstOrDefault()!; } }
        public ApplicationsContainerArtifact Applications { get { return Children.OfType<ApplicationsContainerArtifact>().FirstOrDefault()!; } }
        public PresentationsContainerArtifact Presentations { get { return Children.OfType<PresentationsContainerArtifact>().FirstOrDefault()!; } }
        public SubScopesContainerArtifact SubScopes { get { return Children.OfType<SubScopesContainerArtifact>().FirstOrDefault()!; } }

        public bool CanBeginEdit()
        {
            return !IsDefaultScope();
        }

        /// <summary>
        /// Returns true if this scope is one of the default scopes (Shared or Application)
        /// </summary>
        public bool IsDefaultScope()
        {
            return Name == DEFAULT_SCOPE_SHARED || Name == DEFAULT_SCOPE_APPLICATION;
        }

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }

        public bool Validating(string newName)
        {
            return !string.IsNullOrWhiteSpace(newName) && newName != DEFAULT_SCOPE_SHARED && newName != DEFAULT_SCOPE_APPLICATION;
        }

        protected override WorkspaceArtifactContext? GetOwnContext()
        {
            var namespaceParameters = new Dictionary<string, string>();
            namespaceParameters.Add("ScopeName", Name);
            namespaceParameters.Add("ScopeNamespace", GetResultingNamespace() ?? string.Empty);
            namespaceParameters.Add("ParentScopeNamespace", GetParentScope()?.GetResultingNamespace() ?? string.Empty);
            return new WorkspaceArtifactContext
            {
                Scope = this,
                Namespace = GetResultingNamespace(),
                NamespaceParameters = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(namespaceParameters)
            };
        }

        public ScopeArtifact? GetParentScope()
        {
            if (Parent is SubScopesContainerArtifact subScopesContainer && subScopesContainer.Parent is ScopeArtifact parentScope)
            {
                return parentScope;
            }
            else
            {
                return null;
            }
        }

        public string? GetResultingNamespace()
        {
            if (Parent is ScopesContainerArtifact scopesContainer && scopesContainer.Parent is WorkspaceArtifact workspace)
            {
                var rootNamespace = workspace.RootNamespace;
                if (!string.IsNullOrWhiteSpace(rootNamespace))
                {
                    if (!string.IsNullOrWhiteSpace(NamespacePattern))
                        return $"{rootNamespace}.{NamespacePattern}";
                    else
                        return rootNamespace;
                }
                else
                {
                    return NamespacePattern;
                }
            }
            else if (Parent is SubScopesContainerArtifact subScopesContainer && subScopesContainer.Parent is ScopeArtifact parentScope)
            {
                var parentNamespace = parentScope.GetResultingNamespace();
                if (!string.IsNullOrWhiteSpace(parentNamespace))
                {
                    if (!string.IsNullOrWhiteSpace(NamespacePattern))
                        return $"{parentNamespace}.{NamespacePattern}";
                    else
                        return parentNamespace;
                }
                else
                {
                    return NamespacePattern;
                }
            }
            else
            {
                return NamespacePattern;
            }
        }
    }
}
