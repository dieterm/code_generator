using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Events;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Factories;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Repositories;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Services;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Specifications;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.ViewModels;
using CodeGenerator.Domain.CodeArchitecture;
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
            EnsureEventsContainerExists();
            EnsureRepositoriesContainerExists();
            EnsureServicesContainerExists();
            EnsureSpecificationsContainerExists();
            EnsureFactoriesContainerExists();
            PublishArtifactConstructionEvent();
        }

        

        public DomainArtifact(ArtifactState state)
            : base(state)
        {
            EnsureEntitiesContainerExists();
            EnsureValueTypesContainerExists();
            EnsureEventsContainerExists();
            EnsureRepositoriesContainerExists();
            EnsureServicesContainerExists();
            EnsureSpecificationsContainerExists();
            EnsureFactoriesContainerExists();

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
                    { ScopeArtifact.CONTEXT_PARAMETER_PARENT_SCOPE_NAME, Scope.Name  },
                    { ScopeArtifact.CONTEXT_PARAMETER_SCOPE_NAMESPACE, Scope.Namespace },
                    { CodeArchitectureLayerArtifact.CONTEXT_PARAMETER_LAYER_NAME, (Parent as CodeArchitectureLayerArtifact)?.LayerName ?? "?LayerName?" },
                    { WorkspaceArtifact.CONTEXT_PARAMETER_WORKSPACE_ROOT_NAMESPACE, GetWorkspaceRootNamespace() }
                };

                return paremeteraisedString.GetOutput(parameters);
            }
        }

        private string GetWorkspaceRootNamespace()
        {
            var workspaceContext = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
            return workspaceContext.CurrentWorkspace?.RootNamespace ?? string.Empty;
        }

        /// <summary>
        /// Gets the EntitiesContainerArtifact for this domain
        /// </summary>
        public EntitiesContainerArtifact Entities => EnsureEntitiesContainerExists();

        /// <summary>
        /// Gets the ValueTypesContainerArtifact for this domain
        /// </summary>
        public ValueTypesContainerArtifact ValueTypes => EnsureValueTypesContainerExists();

        public DomainEventsContainerArtifact Events => EnsureEventsContainerExists();
        public DomainServicesContainerArtifact Services => EnsureServicesContainerExists();
        public DomainRepositoriesContainerArtifact Repositories => EnsureRepositoriesContainerExists();
        public DomainSpecificationsContainerArtifact Specifications => EnsureSpecificationsContainerExists();
        public DomainFactoriesContainerArtifact Factories => EnsureFactoriesContainerExists();

        private DomainFactoriesContainerArtifact EnsureFactoriesContainerExists()
        {
            var existing = Children.OfType<DomainFactoriesContainerArtifact>().SingleOrDefault();
            if (existing == null)
            {
                existing = new DomainFactoriesContainerArtifact();
                AddChild(existing);
            }
            return existing;
        }

        private DomainSpecificationsContainerArtifact EnsureSpecificationsContainerExists()
        {
            var existing = Children.OfType<DomainSpecificationsContainerArtifact>().SingleOrDefault();
            if (existing == null)
            {
                existing = new DomainSpecificationsContainerArtifact();
                AddChild(existing);
            }
            return existing;
        }

        private DomainEventsContainerArtifact EnsureEventsContainerExists()
        {
            var existing = Children.OfType<DomainEventsContainerArtifact>().SingleOrDefault();
            if (existing == null)
            {
                existing = new DomainEventsContainerArtifact();
                AddChild(existing);
            }
            return existing;
        }

        private EntitiesContainerArtifact EnsureEntitiesContainerExists()
        {
            var existing = Children.OfType<EntitiesContainerArtifact>().SingleOrDefault();
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
            var existing = Children.OfType<ValueTypesContainerArtifact>().SingleOrDefault();
            if (existing == null)
            {
                existing = new ValueTypesContainerArtifact();
                // Automatically add ValueTypesContainerArtifact
                AddChild(existing);
            }
            return existing;
        }

        private DomainServicesContainerArtifact EnsureServicesContainerExists()
        {
            var existing = Children.OfType<DomainServicesContainerArtifact>().SingleOrDefault();
            if (existing == null)
            {
                existing = new DomainServicesContainerArtifact();
                // Automatically add DomainServicesContainerArtifact
                AddChild(existing);
            }
            return existing;
        }

        private DomainRepositoriesContainerArtifact EnsureRepositoriesContainerExists()
        {
            var existing = Children.OfType<DomainRepositoriesContainerArtifact>().SingleOrDefault();
            if (existing == null)
            {
                existing = new DomainRepositoriesContainerArtifact();
                // Automatically add DomainRepositoriesContainerArtifact
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

        public EntityArtifact AddEntity(EntityArtifact entityArtifact)
        {
           return Entities.AddEntity(entityArtifact);
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
