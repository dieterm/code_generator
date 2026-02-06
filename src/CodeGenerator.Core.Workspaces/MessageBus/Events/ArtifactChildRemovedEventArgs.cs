using CodeGenerator.Core.Workspaces.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.MessageBus.Events
{
    public class ArtifactChildRemovedEventArgs : WorkspaceEventArg
    {
        public WorkspaceArtifactBase ParentArtifact { get; }
        public WorkspaceArtifactBase ChildArtifact { get; }
        public ArtifactChildRemovedEventArgs(WorkspaceArtifactBase parentArtifact, WorkspaceArtifactBase childArtifact)
        {
            ParentArtifact = parentArtifact;
            ChildArtifact = childArtifact;
        }
    }
}
