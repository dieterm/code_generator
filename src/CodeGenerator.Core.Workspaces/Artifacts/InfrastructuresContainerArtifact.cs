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
    public class InfrastructuresContainerArtifact : WorkspaceArtifactBase, ILayerArtifact
    {
        public InfrastructuresContainerArtifact()
        {
            
        }
        public InfrastructuresContainerArtifact(ArtifactState state)
            : base(state)
        {
            
        }
        public override string TreeNodeText => "Infrastructure";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("factory");

        public string LayerName => CodeArchitectureLayerArtifact.INFRASTRUCTURE_LAYER;

        protected override WorkspaceArtifactContext? GetOwnContext()
        {
            return new WorkspaceArtifactContext
            {
                Namespace = (Parent as WorkspaceArtifactBase).Context.Namespace + "." + LayerName
            };
        }
    }
}
