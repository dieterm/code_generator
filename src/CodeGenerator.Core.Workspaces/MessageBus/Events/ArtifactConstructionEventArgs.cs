using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.MessageBus.Events
{
    public class ArtifactConstructionEventArgs : WorkspaceEventArg
    {
        public IArtifact Artifact { get; }

        public ArtifactConstructionEventArgs(IArtifact artifact) 
        {
            Artifact = artifact;
        }
    }

}
