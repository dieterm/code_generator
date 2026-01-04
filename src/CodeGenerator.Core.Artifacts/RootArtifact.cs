using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    /// <summary>
    /// This artifact represents the root of an artifact tree.
    /// </summary>
    public sealed class RootArtifact : Artifact
    {
        public override Task GenerateSelfAsync(CancellationToken cancellationToken = default)
        {
            // Root artifact does not generate any content itself.
            return Task.CompletedTask;
        }
    }
}
