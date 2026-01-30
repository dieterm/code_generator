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

            AddChild(new DomainArtifact("Domain"));
            AddChild(new InfrastructuresContainerArtifact());
            AddChild(new ApplicationsContainerArtifact());
            AddChild(new PresentationsContainerArtifact());

            PublishArtifactConstructionEvent();
        }

        public ScopeArtifact(ArtifactState state)
            : base(state) 
        {
            EnsureChildArtifactExists<DomainArtifact>(() => new DomainArtifact("Domain"));
            EnsureChildArtifactExists<InfrastructuresContainerArtifact>();
            EnsureChildArtifactExists<ApplicationsContainerArtifact>();
            EnsureChildArtifactExists<PresentationsContainerArtifact>();
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
        public string Namespace
        {
            get { return GetValue<string>(nameof(Namespace)); }
            set { SetValue(nameof(Namespace), value); }
        }

        public DomainArtifact Domain { get { return Children.OfType<DomainArtifact>().FirstOrDefault()!; } }

        public bool CanBeginEdit()
        {
            return Name != DEFAULT_SCOPE_SHARED && Name != DEFAULT_SCOPE_APPLICATION;
        }

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }

        public bool Validating(string newName)
        {
            return !string.IsNullOrWhiteSpace(newName) && newName != DEFAULT_SCOPE_SHARED && newName != DEFAULT_SCOPE_APPLICATION;
        }
    }
}
