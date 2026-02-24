using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.HexagonArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.NTierArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.HexagonArchitecture
{
    public class HexagonScopeArtifact : ScopeArtifact
    {
        public HexagonScopeArtifact(string name, IEnumerable<IHexagonArchitectureLayerFactory> layerFactories)
            : base(name, layerFactories)
        {
        }

        public HexagonScopeArtifact(ArtifactState state, List<string> errors) 
            : base(state, errors)
        {
        }

        public HexagonAdaptersLayerArtifact AdaptersLayerArtifact { get { return Children.OfType<HexagonAdaptersLayerArtifact>().SingleOrDefault()!; } }
        public HexagonPortsLayerArtifact PortsLayerArtifact { get { return Children.OfType<HexagonPortsLayerArtifact>().SingleOrDefault()!; } }
        public HexagonCoreLayerArtifact CoreLayerArtifact { get { return Children.OfType<HexagonCoreLayerArtifact>().SingleOrDefault()!; } }
    }
}
