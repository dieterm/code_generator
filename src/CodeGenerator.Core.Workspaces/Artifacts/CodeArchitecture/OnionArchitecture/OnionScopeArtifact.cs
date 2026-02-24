using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.OnionArchitecture
{
    public class OnionScopeArtifact : ScopeArtifact
    {
        public OnionScopeArtifact(string scopeName, IEnumerable<IOnionArchitectureLayerFactory> layerFactories) 
            : base(scopeName, layerFactories)
        {
            
        }

        public OnionScopeArtifact(ArtifactState state, List<string> errors) 
            : base(state, errors)
        {

        }

        public OnionDomainLayerArtifact Domains { get { return Children.OfType<OnionDomainLayerArtifact>().SingleOrDefault()!; } }
        public OnionInfrastructureLayerArtifact Infrastructure { get { return Children.OfType<OnionInfrastructureLayerArtifact>().SingleOrDefault()!; } }
        public OnionApplicationLayerArtifact Applications { get { return Children.OfType<OnionApplicationLayerArtifact>().SingleOrDefault()!; } }
        public OnionPresentationLayerArtifact Presentations { get { return Children.OfType<OnionPresentationLayerArtifact>().SingleOrDefault()!; } }
       
        public DomainArtifact? FindDomain(string domainName, bool exceptionIfNotFound = true)
        {
            return Domains.FindDomain(domainName, exceptionIfNotFound);
        }

    }
}
