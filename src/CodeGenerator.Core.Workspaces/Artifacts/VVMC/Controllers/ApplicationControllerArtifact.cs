using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Core.Workspaces.Artifacts.VVMC.Controllers
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