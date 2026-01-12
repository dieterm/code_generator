using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    public sealed class ArtifactCollectionState
    {
        public List<ArtifactState> Artifacts { get; } = new List<ArtifactState>();
    }
}
