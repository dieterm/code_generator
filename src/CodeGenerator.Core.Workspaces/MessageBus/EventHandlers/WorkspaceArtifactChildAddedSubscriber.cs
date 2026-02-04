using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.MessageBus.EventHandlers
{
    public abstract class WorkspaceArtifactChildAddedSubscriber<TParent, TChild> : IWorkspaceMessageBusSubscriber
        where TParent : WorkspaceArtifactBase
        where TChild : WorkspaceArtifactBase
    {
        private Action<ArtifactChildAddedEventArgs>? _subscriptionToken;

        public void Subscribe(WorkspaceMessageBus messageBus)
        {
            _subscriptionToken = messageBus.Subscribe<ArtifactChildAddedEventArgs>(OnArtifactChildAdded, FilterArtifactType);
        }

        private bool FilterArtifactType(ArtifactChildAddedEventArgs args)
        {
            return args.ParentArtifact is TParent && args.ChildArtifact is TChild;
        }

        private void OnArtifactChildAdded(ArtifactChildAddedEventArgs args)
        {
            HandleArtifactChildAdded(args, (TParent)args.ParentArtifact, (TChild)args.ChildArtifact);
        }

        protected abstract void HandleArtifactChildAdded(ArtifactChildAddedEventArgs args, TParent parentArtifact, TChild childArtifact);

        public void Unsubscribe(WorkspaceMessageBus messageBus)
        {
            if (_subscriptionToken != null)
            {
                messageBus.Unsubscribe<ArtifactChildAddedEventArgs>(_subscriptionToken);
                _subscriptionToken = null;
            }
        }
    }
}
