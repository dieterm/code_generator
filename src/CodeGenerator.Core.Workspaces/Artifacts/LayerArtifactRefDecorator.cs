using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class LayerArtifactRefDecorator : ArtifactDecorator
    {
        public CodeArchitectureLayerArtifact LayerArtifact { get; }

        public LayerArtifactRefDecorator(string key, CodeArchitectureLayerArtifact layerArtifact) : base(key)
        {
            LayerArtifact = layerArtifact ?? throw new ArgumentNullException(nameof(layerArtifact));
        }
    }
}
