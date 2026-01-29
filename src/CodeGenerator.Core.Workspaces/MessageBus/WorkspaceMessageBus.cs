using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.MessageBus
{
    public class WorkspaceMessageBus : MessageBus<WorkspaceEventArg>, IDisposable
    {
        private List<IWorkspaceMessageBusSubscriber> _subscribers = new List<IWorkspaceMessageBusSubscriber>();
        public void Initialize()
        {
            _subscribers.AddRange(ServiceProviderHolder.ServiceProvider.GetServices<IWorkspaceMessageBusSubscriber>());
            _subscribers.ForEach(subscriber => subscriber.Subscribe(this));
        }

        public void PublishArtifactConstruction(WorkspaceArtifact workspaceArtifact, IArtifact artifact)
        {
            Publish(new ArtifactConstructionEventArgs(workspaceArtifact, artifact));
        }

        public void Dispose()
        {
            _subscribers.ForEach(subscriber => subscriber.Unsubscribe(this));
        }
    }
}
