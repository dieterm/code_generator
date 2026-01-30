using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class InfrastructuresContainerArtifact : WorkspaceArtifactBase
    {
        public InfrastructuresContainerArtifact()
        {
            
        }
        public InfrastructuresContainerArtifact(ArtifactState state)
            : base(state)
        {
            
        }
        public override string TreeNodeText => "Infrastructure";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("factory");
    }
}
