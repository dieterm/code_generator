using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.MessageBus.EventHandlers
{
    public abstract class ArtifactConstructionSubscriberBase<TArtifact> : IWorkspaceMessageBusSubscriber where TArtifact : IArtifact
    {
        private Action<ArtifactConstructionEventArgs>? _subscriptionToken;

        public void Subscribe(WorkspaceMessageBus messageBus)
        {
            _subscriptionToken = messageBus.Subscribe<ArtifactConstructionEventArgs>(OnArtifactCreation, FilterArtifactType);
        }

        private void OnArtifactCreation(ArtifactConstructionEventArgs args)
        {
            var artifact = (TArtifact)args.Artifact;
            HandleArtifactCreation(args, artifact);
        }
        protected abstract void HandleArtifactCreation(ArtifactConstructionEventArgs args, TArtifact artifact);

        private bool FilterArtifactType(ArtifactConstructionEventArgs args)
        {
            return args.Artifact is TArtifact;
        }

        public void Unsubscribe(WorkspaceMessageBus messageBus)
        {
            messageBus.Unsubscribe<ArtifactConstructionEventArgs>(_subscriptionToken);
        }
    }
}
