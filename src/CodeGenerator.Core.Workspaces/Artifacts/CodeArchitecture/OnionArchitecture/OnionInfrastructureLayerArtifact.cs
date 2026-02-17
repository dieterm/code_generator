using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class OnionInfrastructureLayerArtifact : CodeArchitectureLayerArtifact
    {
        public OnionInfrastructureLayerArtifact(string initialScopeName)
            : base(OnionCodeArchitecture.INFRASTRUCTURE_LAYER, initialScopeName)
        {
            EnsureChildArtifactExists<InfrastructureRepositoriesContainerArtifact>();
            EnsureChildArtifactExists<InfrastructureServicesContainerArtifact>();
        }

        public OnionInfrastructureLayerArtifact(ArtifactState state, List<string> errors) 
            : base(state, errors)
        {
            EnsureChildArtifactExists<InfrastructureRepositoriesContainerArtifact>();
            EnsureChildArtifactExists<InfrastructureServicesContainerArtifact>();
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("factory");

        public InfrastructureRepositoriesContainerArtifact Repositories => EnsureChildArtifactExists<InfrastructureRepositoriesContainerArtifact>();  
        
        public InfrastructureServicesContainerArtifact Services => EnsureChildArtifactExists<InfrastructureServicesContainerArtifact>();
       
    }
}
