using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class OnionDomainLayerFactory : IOnionArchitectureLayerFactory
    {
        public string LayerName => OnionCodeArchitecture.DOMAIN_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            // Implementation for creating a domain layer artifact based on the scope
            return new OnionDomainLayerArtifact(scope);
        }

    }
}
