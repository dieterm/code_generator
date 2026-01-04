using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Artifacts
{
    public abstract class ArtifactHost
    {
        protected ArtifactHost(Artifact artifact)
        {
            Artifact = artifact;
        }
        protected ArtifactHost()
        {
            Artifact = new Artifact();
        }
        public Artifact Artifact { get; }
    }
}
