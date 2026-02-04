using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.MessageBus.EventHandlers
{
    public class WorkspaceArtifactConstructionSubscriber : ArtifactConstructionSubscriberBase<WorkspaceArtifact>
    {
        protected override void HandleArtifactCreation(ArtifactConstructionEventArgs args, WorkspaceArtifact artifact)
        {
           
        }

    }

}
