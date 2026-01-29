using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;

namespace CodeGenerator.Core.Workspaces.MessageBus
{
    public abstract class WorkspaceEventArg : EventArgs
    {
        public WorkspaceArtifact? WorkspaceArtifact { get; }

        protected WorkspaceEventArg(WorkspaceArtifact? workspaceArtifact)
        {
            if (workspaceArtifact == null)
                workspaceArtifact = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>().CurrentWorkspace;
            //if (workspaceArtifact==null)
                //throw new ArgumentNullException(nameof(workspaceArtifact));
            WorkspaceArtifact = workspaceArtifact;
        }
    }
}