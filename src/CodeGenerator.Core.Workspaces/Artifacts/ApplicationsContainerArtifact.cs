using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class ApplicationsContainerArtifact : WorkspaceArtifactBase
    {
        public override string TreeNodeText => "Application";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("dock");
    }
}
