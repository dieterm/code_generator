using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Plugin.TestPlugin
{
    public class TestWorkspaceSubscriber : ArtifactConstructionSubscriberBase<EntityArtifact>
    {
        private readonly ILogger _logger;

        public TestWorkspaceSubscriber(ILogger logger)
        {
            _logger = logger;
        }

        protected override void HandleArtifactCreation(ArtifactConstructionEventArgs args, EntityArtifact artifact)
        {
            _logger.LogInformation("TestPlugin: Entity artifact created: {ArtifactName}", artifact.Name);
        }
    }
}