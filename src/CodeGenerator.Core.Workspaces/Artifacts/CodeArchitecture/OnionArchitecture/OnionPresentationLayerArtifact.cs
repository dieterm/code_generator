using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.VVMC;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class OnionPresentationLayerArtifact : CodeArchitectureLayerArtifact
    {
        public OnionPresentationLayerArtifact(string initialScopeName) 
            : base(OnionCodeArchitecture.PRESENTATION_LAYER, initialScopeName)
        {
            
        }

        public OnionPresentationLayerArtifact(ArtifactState state) 
            : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("presentation");
    }
}
