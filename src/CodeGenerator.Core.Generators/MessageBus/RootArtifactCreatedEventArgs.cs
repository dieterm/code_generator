using CodeGenerator.Core.Artifacts.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Generators.MessageBus
{
    public class RootArtifactCreatedEventArgs : GeneratorContextEventArgs
    {
        public RootArtifact RootArtifact { get; }

        public RootArtifactCreatedEventArgs(RootArtifact RootArtifact, GenerationResult result) : base(result)
        {
            this.RootArtifact = RootArtifact;
        }
    }
}
