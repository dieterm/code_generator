using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public class DotNetProjectReference
    {
        public DotNetProjectReference(DotNetProjectArtifact projectArtifact)
        {
            ProjectArtifact = projectArtifact;
        }
        public DotNetProjectArtifact ProjectArtifact { get; }
    }
}
