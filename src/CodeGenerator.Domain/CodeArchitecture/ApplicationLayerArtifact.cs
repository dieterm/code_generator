using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class ApplicationLayerArtifact : CodeArchitectureLayerArtifact
    {
        public ApplicationLayerArtifact(string scope) : base(CodeArchitectureLayerArtifact.APPLICATION_LAYER, scope)
        {
        }
    }
}
