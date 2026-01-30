using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.MessageBus.Events
{
    public class ArtifactContextMenuOpeningEventArgs : WorkspaceEventArg
    {
        public IArtifact Artifact { get; }
        public List<ArtifactTreeNodeCommand> Commands { get; } 
        public ArtifactContextMenuOpeningEventArgs(IArtifact artifact, List<ArtifactTreeNodeCommand> commands)
        {
            Artifact = artifact ?? throw new ArgumentNullException(nameof(artifact));
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }
        
    }
}
