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
    public class PresentationsContainerArtifact : WorkspaceArtifactBase, ILayerArtifact
    {
        public PresentationsContainerArtifact()
        {

        }
        public PresentationsContainerArtifact(ArtifactState state)
            : base(state)
        {

        }
        public override string TreeNodeText => "Presentation";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("presentation");

        public string LayerName => CodeArchitectureLayerArtifact.PRESENTATION_LAYER;

        protected override WorkspaceArtifactContext? GetOwnContext()
        {
            return new WorkspaceArtifactContext
            {
                Namespace = (Parent as WorkspaceArtifactBase).Context.Namespace + "." + LayerName
            };
        }
    }
}
