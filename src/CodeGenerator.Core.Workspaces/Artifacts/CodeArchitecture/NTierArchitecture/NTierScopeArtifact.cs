using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.NTierArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.NTierArchitecture
{
    public class NTierScopeArtifact : ScopeArtifact
    {
        public NTierScopeArtifact(string name, IEnumerable<INTierArchitectureLayerFactory> layerFactories) 
            : base(name, layerFactories)
        {
            
        }

        public NTierScopeArtifact(ArtifactState state, List<string> errors) 
            : base(state, errors)
        {

        }

        public NTierBusinessLayerArtifact BusinessLayer { get { return Children.OfType<NTierBusinessLayerArtifact>().SingleOrDefault()!; } }
        public NTierDataAccessLayerArtifact DataAccessLayer { get { return Children.OfType<NTierDataAccessLayerArtifact>().SingleOrDefault()!; } }
        public NTierPresentationLayerArtifact PresentationLayer { get { return Children.OfType<NTierPresentationLayerArtifact>().SingleOrDefault()!; } }

    }
}
