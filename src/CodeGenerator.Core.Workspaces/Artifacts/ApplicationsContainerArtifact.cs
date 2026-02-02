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
    public class ApplicationsContainerArtifact : WorkspaceArtifactBase, ILayerArtifact
    {
        public ApplicationsContainerArtifact()
        {

        }
        public ApplicationsContainerArtifact(ArtifactState state)
            : base(state)
        {

        }
        public override string TreeNodeText => "Application";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("dock");

        public string LayerName => CodeArchitectureLayerArtifact.APPLICATION_LAYER;

        protected override WorkspaceArtifactContext? GetOwnContext()
        {
            return new WorkspaceArtifactContext
            {
                Namespace = (Parent as WorkspaceArtifactBase).Context.Namespace + "." + LayerName
            };
        }
    }
}
