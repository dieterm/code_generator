using CodeGenerator.Core.Generators;
using CodeGenerator.Generators.DotNet.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Events
{
    public class DiExtensionsClassArtifactCreatedEventArgs : GeneratorContextEventArgs
    {
        public DiExtensionsClassArtifact DiExtensionsClassArtifact { get; }
        public string Layer { get; }
        public string Scope { get; }
        public DiExtensionsClassArtifactCreatedEventArgs(DiExtensionsClassArtifact diExtensionsClassArtifact, string layer, string scope, GenerationResult result) 
            : base(result)
        {
            DiExtensionsClassArtifact = diExtensionsClassArtifact;
            Layer = layer;
            Scope = scope;
        }
    }
}
