using CodeGenerator.Core.Generators;
using CodeGenerator.Domain.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Events
{
    public class DotNetProjectArtifactCreatedEventArgs : GeneratorContextEventArgs
    {
        public DotNetProjectArtifact DotNetProjectArtifact { get; }
        public string Layer { get; }
        public string Scope { get; }

        public DotNetProjectArtifactCreatedEventArgs(GenerationResult result, DotNetProjectArtifact dotNetProjectArtifact, string layer, string scope) 
            : base(result)
        {
            DotNetProjectArtifact = dotNetProjectArtifact;
            Layer = layer;
            Scope = scope;
        }
    }
}
