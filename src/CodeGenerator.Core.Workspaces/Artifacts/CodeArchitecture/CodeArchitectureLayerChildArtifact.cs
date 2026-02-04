using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture
{
    public abstract class CodeArchitectureLayerChildArtifact : WorkspaceArtifactBase
    {
        public CodeArchitectureLayerChildArtifact()
        {
        }

        public CodeArchitectureLayerChildArtifact(ArtifactState state)
            : base(state)
        {
        }

        public CodeArchitectureLayerArtifact Layer
        {
            get
            {
                if (Parent is CodeArchitectureLayerArtifact parentLayer)
                    return parentLayer;
                else if (Parent is CodeArchitectureLayerChildArtifact layerChildArtifact)
                    return layerChildArtifact.Layer;
                else
                    throw new InvalidOperationException("CodeArchitectureLayerChildArtifact must have a CodeArchitectureLayerArtifact or CodeArchitectureLayerChildArtifact as parent.");
            }
        }
    }
}
