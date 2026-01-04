using CodeGenerator.Domain.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Tests
{
    public class ArtifactCreating : EventArgs
    {
        public ArtifactCreating(Artifact artifact)
        {
            Artifact = artifact;
        }
        public Artifact Artifact { get; }
    }
}
