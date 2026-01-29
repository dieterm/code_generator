using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
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
        public const string DEFAULT_DOMAIN_NAME_APPLICATION = "Application";
        public const string DEFAULT_DOMAIN_NAME_SHARED = "Shared";

        public DomainArtifact(string name)
            : base()
        {
            Name = name;
            
            EnsureEntitiesContainerExists();
            EnsureValueTypesContainerExists();
            
            PublishArtifactCreationEvent();
        }



        public DomainArtifact(ArtifactState state)
            : base(state)
        {
            EnsureEntitiesContainerExists();
            EnsureValueTypesContainerExists();

            PublishArtifactCreationEvent();
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
    }
}
