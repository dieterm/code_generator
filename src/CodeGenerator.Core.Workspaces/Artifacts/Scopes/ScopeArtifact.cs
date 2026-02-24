using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Workspace;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.Views.TreeNode;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGenerator.Core.Workspaces.Artifacts.Scopes
{
    public abstract class ScopeArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public const string SCOPE_FULL_NAME_SEPERATOR = "/";
        public const string CONTEXT_PARAMETER_SCOPE_NAME = "ScopeName";
        public const string CONTEXT_PARAMETER_SCOPE_NAMESPACE = "ScopeNamespace";
        public const string CONTEXT_PARAMETER_PARENT_SCOPE_NAME = "ParentScopeName";
        public const string CONTEXT_PARAMETER_PARENT_SCOPE_NAMESPACE = "ParentScopeNamespace";

        public const string DEFAULT_SCOPE_SHARED = "Shared";
        public static Color DEFAULT_SCOPE_SHARED_COLOR = Color.Blue;

        public const string DEFAULT_SCOPE_APPLICATION = "Application";
        public static Color DEFAULT_SCOPE_APPLICATION_COLOR = Color.Green;
        private bool _setNamespacePatternOnFirstRead = false;
        public ScopeArtifact(string name, IEnumerable<ICodeArchitectureLayerFactory> layerFactories)
            : base()
        {
            Name = name;
            // we cannot set a default NamespacePattern in the constructor as it depends on the parent scope (which is not known at construction time)
            // so we use a flag to set it on first read
            _setNamespacePatternOnFirstRead = true;

            NamespacePattern = $"{{{WorkspaceArtifact.CONTEXT_PARAMETER_WORKSPACE_ROOT_NAMESPACE}}}.{{{CONTEXT_PARAMETER_SCOPE_NAME}}}";
            
            foreach (var layerFactory in layerFactories)
            {
                AddChild(layerFactory.CreateLayer(name));
            }
            AddChild(new SubScopesContainerArtifact());

            PublishArtifactConstructionEvent();
        }

        public ScopeArtifact? FindScopeRecursive(string scopeName)
        {
            foreach (var subScope in SubScopes)
            {
                if (subScope.Name.Equals(scopeName, StringComparison.OrdinalIgnoreCase))
                    return subScope;
                var found = subScope.FindScopeRecursive(scopeName);
                if (found != null)
                    return found;
            }
            return null;
        }

        public ScopeArtifact(ArtifactState state, List<string> errors)
            : base(state, errors)
        {
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
                if (SetValue<string>(nameof(Name), value))
                {
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                    RaisePropertyChangedEvent(nameof(FullName));
                    RaisePropertyChangedEvent(nameof(Namespace));
                    RaiseContextChanged();
                }
            }
        }
        /// <summary>
        /// Display full name of the scope including parent scopes separated by SCOPE_FULL_NAME_SEPERATOR. 
        /// E.g. "ParentScope/ChildScope"
        /// </summary>
        public string FullName
        {
            get
            {
                if (GetParentScope() == null)
                    return Name;
                else
                    return $"{GetParentScope()!.FullName}{SCOPE_FULL_NAME_SEPERATOR}{Name}";
            }
        }

        /// <summary>
        /// Default namespace of the scope
        /// </summary>
        public string NamespacePattern
        {
            get
            {
                // we cannot set a default value in the constructor as it depends on the parent scope (which is not known at construction time)
                // so we use a flag to set it on first read
                if (_setNamespacePatternOnFirstRead)
                {
                    _setNamespacePatternOnFirstRead = false;
                    if (GetParentScope() == null)
                        NamespacePattern = $"{{{WorkspaceArtifact.CONTEXT_PARAMETER_WORKSPACE_ROOT_NAMESPACE}}}.{{{CONTEXT_PARAMETER_SCOPE_NAME}}}";
                    else
                        NamespacePattern = $"{{{CONTEXT_PARAMETER_PARENT_SCOPE_NAMESPACE}}}.{{{CONTEXT_PARAMETER_SCOPE_NAME}}}";
                }
                return GetValue<string>(nameof(NamespacePattern));
            }
            set { SetValue(nameof(NamespacePattern), value); }
        }
        private bool _isGettingOwnNamespace = false;
        /// <summary>
        /// The resulting namespace after parameterization
        /// </summary>
        public string Namespace
        {
            get
            {
                _isGettingOwnNamespace = true;
                var parameterisedString = new ParameterizedString(NamespacePattern);
                var output = parameterisedString.GetOutput(Context.NamespaceParameters);
                _isGettingOwnNamespace = false;
                return output;
            }
        }


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
            namespaceParameters.Add(CONTEXT_PARAMETER_SCOPE_NAME, Name);
            if (!_isGettingOwnNamespace)
            {
                namespaceParameters.Add(CONTEXT_PARAMETER_SCOPE_NAMESPACE, Namespace);
            }
            namespaceParameters.Add(CONTEXT_PARAMETER_PARENT_SCOPE_NAME, GetParentScope()?.Name ?? string.Empty);
            namespaceParameters.Add(CONTEXT_PARAMETER_PARENT_SCOPE_NAMESPACE, GetParentScope()?.Namespace ?? string.Empty);

            return new WorkspaceArtifactContext
            {
                Scope = this,
                Namespace = _isGettingOwnNamespace ? null : Namespace,
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

        public TScope? GetParentScope<TScope>() where TScope : ScopeArtifact
        {
            if (Parent is SubScopesContainerArtifact subScopesContainer && subScopesContainer.Parent is TScope parentScope)
            {
                return parentScope;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Solution sub-folder for this scope used to organize projects in visual studio solution file
        /// </summary>
        /// <returns></returns>
        public string GetSolutionSubFolder()
        {
            if (GetParentScope() == null)
                return Name;
            else
                return $"{GetParentScope()!.GetSolutionSubFolder()}\\{Name}";
        }

        public IEnumerable<CodeArchitectureLayerArtifact> GetLayers()
        {
            return Children.OfType<CodeArchitectureLayerArtifact>().ToArray();
        }

        public ScopeArtifact EnsureScope(string scopeName, bool caseInsensitive = true) 
        {
            var existingScope = FindScope(scopeName, caseInsensitive);
            if (existingScope != null)
                return existingScope;
            return SubScopes.AddScope(scopeName);
        }

        public TScope EnsureScope<TScope>(string scopeName, bool caseInsensitive = true) where TScope : ScopeArtifact
        {
            var existingScope = FindScope<TScope>(scopeName, caseInsensitive);
            if (existingScope != null)
                return existingScope;
            return SubScopes.AddScope<TScope>(scopeName);
        }

        public ScopeArtifact? FindScope(string scopeName, bool caseInsensitive = true)
        {
            if (scopeName.Contains(ScopeArtifact.SCOPE_FULL_NAME_SEPERATOR))
            {
                // Handle full name search
                throw new InvalidOperationException("Finding scope by full name is not implemented yet. ");
            }

            return SubScopes.FirstOrDefault(s => s.Name.Equals(scopeName, caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        }

        

        public TScope? FindScope<TScope>(string scopeName, bool caseInsensitive = true) where TScope : ScopeArtifact
        {
            if (scopeName.Contains(ScopeArtifact.SCOPE_FULL_NAME_SEPERATOR))
            {
                // Handle full name search
                throw new InvalidOperationException("Finding scope by full name is not implemented yet. ");
            }
            return SubScopes.OfType<TScope>().FirstOrDefault(s => s.Name.Equals(scopeName, caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        }
    }
}