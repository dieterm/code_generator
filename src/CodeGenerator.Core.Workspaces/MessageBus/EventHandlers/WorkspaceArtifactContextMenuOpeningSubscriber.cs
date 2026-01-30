using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.MessageBus.EventHandlers
{
    public abstract class WorkspaceArtifactContextMenuOpeningSubscriber<TArtifact> : IWorkspaceMessageBusSubscriber where TArtifact : IArtifact
    {
        private Action<ArtifactContextMenuOpeningEventArgs>? _subscriptionToken;
        public void Subscribe(WorkspaceMessageBus messageBus)
        {
            _subscriptionToken = messageBus.Subscribe<ArtifactContextMenuOpeningEventArgs>(OnArtifactContextMenuOpening, FilterArtifactType);
        }

        protected virtual bool FilterArtifactType(ArtifactContextMenuOpeningEventArgs e)
        {
            return e.Artifact is TArtifact;
        }

        protected virtual void OnArtifactContextMenuOpening(ArtifactContextMenuOpeningEventArgs e)
        {
            var artifact = (TArtifact)e.Artifact;
            HandleArtifactContextMenuOpening(e, artifact);
        }

        protected abstract void HandleArtifactContextMenuOpening(ArtifactContextMenuOpeningEventArgs args, TArtifact artifact);

        public void Unsubscribe(WorkspaceMessageBus messageBus)
        {
            messageBus.Unsubscribe(_subscriptionToken!);
        }
    }
}
