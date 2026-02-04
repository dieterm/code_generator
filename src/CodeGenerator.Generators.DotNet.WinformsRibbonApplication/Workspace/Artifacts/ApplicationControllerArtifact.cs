using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.VVMC;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication.Workspace.Artifacts
{
    public class ApplicationControllerArtifact : ControllerArtifact
    {
        public ApplicationControllerArtifact()
            : base("ApplicationController")
        {
        }
        public ApplicationControllerArtifact(ArtifactState state)
            : base(state)
        {
        }
    }
}