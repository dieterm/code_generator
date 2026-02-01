using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
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
        
        protected void PublishArtifactConstructionEvent()
        {
            var messageBus = ServiceProviderHolder.GetRequiredService<WorkspaceMessageBus>();
            
            messageBus.PublishArtifactConstruction( this);
        }

        /// <summary>
        /// Notify any subscribers that the context menu is opening for this artifact.<br />
        /// Subscribers can add custom commands to the context menu via the returned event args.
        /// </summary>
        public ArtifactContextMenuOpeningEventArgs PublishArtifactContextMenuOpeningEvent(List<ArtifactTreeNodeCommand> commands)
        {
            var messageBus = ServiceProviderHolder.GetRequiredService<WorkspaceMessageBus>();

            var eventArgs = messageBus.PublishArtifactContextMenuOpening(this, commands);

            return eventArgs;
        }

        public T EnsureChildArtifactExists<T>(Func<T>? factory = null, Func<T, bool>? predicate = null) where T : WorkspaceArtifactBase
        {
            if(factory == null) factory = () => Activator.CreateInstance<T>();
            if(predicate == null) predicate = (a) => true;
            var childArtifact = Children.OfType<T>().FirstOrDefault(predicate);
            if (childArtifact == null)
            {
                childArtifact = factory();
                AddChild(childArtifact);
            }
            return childArtifact;
        }

        public WorkspaceArtifact GetWorkspace()
        {
            return FindAncesterOfType<WorkspaceArtifact>() 
                ?? throw new InvalidOperationException("Artifact is not part of a workspace.");
        }
    }
}
