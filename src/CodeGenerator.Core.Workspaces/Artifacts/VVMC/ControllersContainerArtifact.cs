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
    public class ControllersContainerArtifact : CodeArchitectureLayerChildArtifact
    {
        public override string TreeNodeText => "Controllers";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("tower-control");

        public ControllersContainerArtifact()
        {
            
        }

        public ControllersContainerArtifact(ArtifactState state)
            : base(state)
        {
        }
    }
}
