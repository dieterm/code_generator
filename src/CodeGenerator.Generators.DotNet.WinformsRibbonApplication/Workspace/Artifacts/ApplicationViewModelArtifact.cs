using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.VVMC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication.Workspace.Artifacts
{
    public class ApplicationViewModelArtifact : ViewModelArtifact
    {
        public ApplicationViewModelArtifact()
            : base("ApplicationViewModel")
        {
        }

        public ApplicationViewModelArtifact(ArtifactState state)
            : base(state)
        {
        }
    }
}
