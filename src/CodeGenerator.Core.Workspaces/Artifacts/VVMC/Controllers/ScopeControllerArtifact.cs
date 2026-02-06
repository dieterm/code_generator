using CodeGenerator.Core.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.VVMC.Controllers
{
    public class ScopeControllerArtifact : ControllerArtifact
    {
        public ScopeControllerArtifact() : base("ScopeController")
        {
        }

        public ScopeControllerArtifact(ArtifactState state) : base(state)
        {
        }
    }
}
