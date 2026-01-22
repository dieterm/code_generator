using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class DomainLayerArtifact : CodeArchitectureLayerArtifact
    {
        public DomainLayerArtifact(string scope) : base(CodeArchitectureLayerArtifact.DOMAIN_LAYER, scope)
        {
        }
    }
}
