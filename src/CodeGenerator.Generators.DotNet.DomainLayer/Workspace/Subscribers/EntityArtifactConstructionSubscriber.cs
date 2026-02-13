using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Decorators;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using CodeGenerator.Generators.DotNet.DomainLayer.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.DomainLayer.Workspace.Subscribers
{
    public class EntityArtifactConstructionSubscriber : ArtifactConstructionSubscriberBase<EntityArtifact>
    {
        protected override void HandleArtifactCreation(ArtifactConstructionEventArgs args, EntityArtifact artifact)
        {
            if(!artifact.HasDecorator<EntityTemplateArtifactDecorator>())
            {
                artifact.AddDecorator(new EntityTemplateArtifactDecorator(EntityTemplateArtifactDecorator.DECORATOR_KEY) { TemplateId = EntityTemplateGenerator.ENTITY_TEMPLATE_ID });
            }
        }
    }
}
