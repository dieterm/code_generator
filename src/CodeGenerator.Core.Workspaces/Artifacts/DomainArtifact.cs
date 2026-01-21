using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Views.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class DomainArtifact : Artifact, IEditableTreeNode
    {
        public DomainArtifact(string name)
            : base()
        {
            Name = name;
        }

        public DomainArtifact(ArtifactState state)
            : base(state)
        {

        }

        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon { get;  } = new ResourceManagerTreeNodeIcon("box");

        /// <summary>
        /// Display name of the domain
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
        /// Description of the domain
        /// </summary>
        public string Description
        {
            get { return GetValue<string>(nameof(Description)); }
            set { SetValue<string>(nameof(Description), value); }
        }
        
        /// <summary>
        /// Default namespace of the domain
        /// </summary>
        public string DefaultNamespace
        {
            get { return GetValue<string>(nameof(DefaultNamespace)); }
            set { SetValue<string>(nameof(DefaultNamespace), value); }
        }

        public bool CanBeginEdit()
        {
            return Parent!=null;
        }

        public void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }

        public bool Validating(string newName)
        {
            return !string.IsNullOrWhiteSpace(newName);
        }
    }
}
