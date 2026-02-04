using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.VVMC
{
    public class ControllerArtifact : CodeArchitectureLayerChildArtifact
    {
        public override string TreeNodeText => Name;

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("ship-wheel");

        public ControllerArtifact(string controllerName)
        {
            Name = controllerName;
        }

        public ControllerArtifact(ArtifactState state)
            : base(state)
        {
        }

        public string? Name
        {
            get { return GetValue<string>(nameof(Name)); }
            set { 
                if(SetValue<string>(nameof(Name), value)) { 
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                }
            }
        }

    }
}
