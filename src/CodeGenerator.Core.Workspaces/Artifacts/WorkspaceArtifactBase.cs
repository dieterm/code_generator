using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public abstract class WorkspaceArtifactBase : Artifact
    {
        public WorkspaceArtifactBase()
        {
            
        }
        protected WorkspaceArtifactBase(ArtifactState state)
            : base(state)
        {

        }
        private WorkspaceArtifact _workspaceArtifact;
        protected void PublishArtifactCreationEvent()
        {
            var messageBus = ServiceProviderHolder.GetRequiredService<WorkspaceMessageBus>();
            // when construction of a WorkspaceArtifact is complete, store this as the new _workspaceArtifact to use for all subsequent events
            // we cannot use IWorkspaceContextProvider because during the creation of the workspace-tree, the context provider may not yet be fully initialized
            if (this is WorkspaceArtifact workspaceArtifact) 
            { 
                _workspaceArtifact = workspaceArtifact;
            }
            messageBus.PublishArtifactConstruction(_workspaceArtifact, this);
        }
    }
}
