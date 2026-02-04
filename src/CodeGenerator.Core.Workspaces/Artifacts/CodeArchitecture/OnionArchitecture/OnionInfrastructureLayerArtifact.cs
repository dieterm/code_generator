using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
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
        }

        public OnionInfrastructureLayerArtifact(ArtifactState state) 
            : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("factory");
    }
}
