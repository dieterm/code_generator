using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class InfrastructureLayerFactory : ICodeArchitectureLayerFactory
    {
        public CodeArchitectureLayerArtifact CreateLayer(string scope)
        {
            // Implementation for creating an infrastructure layer artifact based on the scope
            return new InfrastructureLayerArtifact(scope);
        }
    }
}
