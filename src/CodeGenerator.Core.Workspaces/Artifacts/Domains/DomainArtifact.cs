using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.Views.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    public class DomainArtifact : Artifact, IEditableTreeNode
    {
        public DomainArtifact(string name)
            : base()
        {
            Name = name;
            
            EnsureEntitiesContainerExists();
        }



        public DomainArtifact(ArtifactState state)
            : base(state)
        {
            EnsureEntitiesContainerExists();
        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("box");

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
        public string DefaultNamespacePattern
        {
            get { return GetValue<string>(nameof(DefaultNamespacePattern)); }
            set { SetValue(nameof(DefaultNamespacePattern), value); }
        }

        public string DefaultNamespace         
        {
            get
            {
                var namespacePattern = DefaultNamespacePattern;
                if (string.IsNullOrWhiteSpace(namespacePattern))
                    return string.Empty;

                var paremeteraisedString = new ParameterizedString(namespacePattern);
                var parameters = new Dictionary<string, string>
                {
                    { DomainEditViewModel.DEFAULT_NAMESPACE_PARAMETER_DOMAIN_NAME, Name },
                    { DomainEditViewModel.DEFAULT_NAMESPACE_PARAMETER_WORKSPACE_ROOT_NAMESPACE, GetWorkspaceRootNamespace() }
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

        public bool CanBeginEdit()
        {
            return Parent != null;
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
    }
}
