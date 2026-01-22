using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class InfrastructureLayerArtifact : CodeArchitectureLayerArtifact
    {
        public InfrastructureLayerArtifact(string scope) : base(CodeArchitectureLayerArtifact.INFRASTRUCTURE_LAYER, scope)
        {
        }
    }
}
