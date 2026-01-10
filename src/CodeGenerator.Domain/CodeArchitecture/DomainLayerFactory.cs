using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class DomainLayerFactory : ICodeArchitectureLayerFactory
    {
        public CodeArchitectureLayerArtifact CreateLayer(string scope)
        {
            // Implementation for creating a domain layer artifact based on the scope
            return new DomainLayerArtifact(scope);
        }
    }
}
