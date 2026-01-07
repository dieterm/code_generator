using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class PresentationLayerArtifact : CodeArchitectureLayerArtifact
    {
        public PresentationLayerArtifact(string scope) : base(CodeArchitectureLayerArtifact.PRESENTATION_LAYER, scope)
        {
        }
    }
}
