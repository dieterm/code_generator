using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication
{
    public class WinformsPresentationArtifact : WorkspaceArtifactBase
    {
        public WinformsPresentationArtifact()
        {

        }
        public WinformsPresentationArtifact(ArtifactState state)
            : base(state)
        {

        }
        public override string TreeNodeText => "Winforms Presentation";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("presentation");
    }
}
