using CodeGenerator.Core.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Generators.MessageBus
{
    public class CreatedArtifactEventArgs : GeneratorContextEventArgs
    {
        public CreatedArtifactEventArgs(GenerationResult result, IArtifact artifact) : base(result)
        {
            Artifact = artifact;
        }
        public IArtifact Artifact { get; }
    }
}
