using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.HexagonArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.CleanArchitecture
{
    public class CleanCodeScopeArtifact : ScopeArtifact
    {
        public CleanCodeScopeArtifact(string name, IEnumerable<ICleanArchitectureLayerFactory> layerFactories) 
            : base(name, layerFactories)
        {
        }

        public CleanCodeScopeArtifact(ArtifactState state, List<string> errors) 
            : base(state, errors)
        {
        }

        public CleanEntitiesLayerArtifact Adapters { get { return Children.OfType<CleanEntitiesLayerArtifact>().SingleOrDefault()!; } }
        public CleanUseCasesLayerArtifact UseCases { get { return Children.OfType<CleanUseCasesLayerArtifact>().SingleOrDefault()!; } }
        public CleanInterfaceAdaptersLayerArtifact Interfaces { get { return Children.OfType<CleanInterfaceAdaptersLayerArtifact>().SingleOrDefault()!; } }
        public CleanFrameworksLayerArtifact FrameworksAndDrivers { get { return Children.OfType<CleanFrameworksLayerArtifact>().SingleOrDefault()!; } }
    }
}
