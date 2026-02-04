using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.Views.TreeNode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    public class DomainArtifact : WorkspaceArtifactBase, IEditableTreeNode
    {
        public const string CONTEXT_PARAMETER_DOMAIN_NAME = "DomainName";
        public const string DEFAULT_DOMAIN_NAME_APPLICATION = "Application";
        public const string DEFAULT_DOMAIN_NAME_SHARED = "Shared";

        public DomainArtifact(string name)
            : base()
        {
            Name = name;
            NamespacePattern = $"{{{ScopeArtifact.CONTEXT_PARAMETER_SCOPE_NAMESPACE}}}.{{{CONTEXT_PARAMETER_DOMAIN_NAME}}}";

            EnsureEntitiesContainerExists();
            EnsureValueTypesContainerExists();
            
            PublishArtifactConstructionEvent();
        }



        public DomainArtifact(ArtifactState state)
            : base(state)
        {
            EnsureEntitiesContainerExists();
            EnsureValueTypesContainerExists();

            PublishArtifactConstructionEvent();
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("box");

        public override Color? TreeNodeTextColor { get { return Name == DEFAULT_DOMAIN_NAME_APPLICATION ? Color.Blue : (Name == DEFAULT_DOMAIN_NAME_SHARED ? Color.Green : (Color?)null); } }
        /// <summary>
        /// Display name of the domain
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(nameof(Name)); }
            set
            {
                SetValue(nameof(Name), value);
                RaisePropertyChangedEvent(nameof(TreeNodeText));
                RaiseContextChanged();
            }
        }

        /// <summary>
        /// Description of the domain
        /// </summary>
        public string Description
        {
            get { return GetValue<string>(nameof(Description)); }
            set { SetValue(nameof(Description), value); }
        }

        /// <summary>
        /// Default namespace of the domain
        /// </summary>
        public string NamespacePattern
        {
            get { return GetValue<string>(nameof(NamespacePattern)); }
            set { SetValue(nameof(NamespacePattern), value); }
        }

        public string Namespace         
        {
            get
            {
                var namespacePattern = NamespacePattern;
                if (string.IsNullOrWhiteSpace(namespacePattern))
                    return string.Empty;

                var paremeteraisedString = new ParameterizedString(namespacePattern);
                var parameters = new Dictionary<string, string>
                {
                    { DomainArtifact.CONTEXT_PARAMETER_DOMAIN_NAME, Name },
                    { WorkspaceArtifact.CONTEXT_PARAMETER_WORKSPACE_ROOT_NAMESPACE, GetWorkspaceRootNamespace() }
                };

                return paremeteraisedString.GetOutput(parameters);
            }
        }

        private string GetWorkspaceRootNamespace()
        {
            var workspaceContext = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
            return workspaceContext.CurrentWorkspace?.RootNamespace ?? "MyCompany.MyProduct";
        }

        /// <summary>
        /// Gets the EntitiesContainerArtifact for this domain
        /// </summary>
        public EntitiesContainerArtifact Entities => EnsureEntitiesContainerExists();

        /// <summary>
        /// Gets the ValueTypesContainerArtifact for this domain
        /// </summary>
        public ValueTypesContainerArtifact ValueTypes => EnsureValueTypesContainerExists();

        private EntitiesContainerArtifact EnsureEntitiesContainerExists()
        {
            var existing = Children.OfType<EntitiesContainerArtifact>().FirstOrDefault();
            if (existing == null)
            {
                existing = new EntitiesContainerArtifact();
                // Automatically add EntitiesContainerArtifact
                AddChild(existing);
            }
            return existing;
        }

        private ValueTypesContainerArtifact EnsureValueTypesContainerExists()
        {
            var existing = Children.OfType<ValueTypesContainerArtifact>().FirstOrDefault();
            if (existing == null)
            {
                existing = new ValueTypesContainerArtifact();
                // Automatically add ValueTypesContainerArtifact
                AddChild(existing);
            }
            return existing;
        }

        public bool CanBeginEdit()
        {
            return Parent != null && Name != DEFAULT_DOMAIN_NAME_APPLICATION && Name != DEFAULT_DOMAIN_NAME_SHARED;
        }

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }

        public bool Validating(string newName)
        {
            return !string.IsNullOrWhiteSpace(newName);
        }

        public void AddEntity(EntityArtifact entityArtifact)
        {
            Entities.AddEntity(entityArtifact);
        }

        public void AddValueType(ValueTypeArtifact valueTypeArtifact)
        {
            ValueTypes.AddValueType(valueTypeArtifact);
        }

        protected override WorkspaceArtifactContext? GetOwnContext()
        {
            var namespaceParameters = new Dictionary<string, string>
            {
                { CONTEXT_PARAMETER_DOMAIN_NAME, Name },
            };
            return new WorkspaceArtifactContext
            {
                Namespace = Namespace,
                NamespaceParameters = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(namespaceParameters)
            };
        }

        public ScopeArtifact? Scope
        {
            get { 
                if (Parent is OnionDomainLayerArtifact domainsContainer && domainsContainer.Parent is ScopeArtifact scope)
                    return scope;
                return null;
            }
        }
    }
}
