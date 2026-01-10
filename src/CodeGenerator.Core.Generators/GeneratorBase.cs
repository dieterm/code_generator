using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Generators
{
    public abstract class GeneratorBase : IMessageBusAwareGenerator
    {
        public GeneratorMessageBus? MessageBus { get; private set; }
        public GeneratorSettingsDescription SettingsDescription { get; set; }

        protected GeneratorBase()
        {
            SettingsDescription = ConfigureSettingsDescription();
        }

        protected abstract GeneratorSettingsDescription ConfigureSettingsDescription();

        protected virtual GeneratorSettings GetSettings()
        {
            var settingsManager = ServiceProviderHolder.GetRequiredService<GeneratorSettingsManager>();
            var settings = settingsManager.GetGeneratorSettings(this.SettingsDescription.Id);
            return settings;
        }

        public virtual void Initialize(GeneratorMessageBus messageBus)
        {
            MessageBus = messageBus;
        }

        public abstract void SubscribeToEvents(GeneratorMessageBus messageBus);

        public abstract void UnsubscribeFromEvents(GeneratorMessageBus messageBus);

        /// <summary>
        /// Publish CreatingArtifactEvent -> Add child to parent -> Publish CreatedArtifactEvent
        /// </summary>
        protected void AddChildArtifactToParent(IArtifact parent, IArtifact child, GenerationResult result)
        {
            MessageBus.Publish(new CreatingArtifactEventArgs(result, child));
            parent.AddChild(child);
            MessageBus.Publish(new CreatedArtifactEventArgs(result, child));
            VisitArtifactChildrenAndPublishCreatingEvents(child, result);
            VisitArtifactChildrenAndPublishCreatedEvents(child, result);
        }

        protected void VisitArtifactChildrenAndPublishCreatingEvents(IArtifact artifact, GenerationResult result)
        {
            foreach (var child in artifact.Children)
            {
                NotifyArtifactCreating(child, result);
                VisitArtifactChildrenAndPublishCreatingEvents(child, result);
            }
        }

        protected void VisitArtifactChildrenAndPublishCreatedEvents(IArtifact artifact, GenerationResult result)
        {
            foreach (var child in artifact.Children)
            {
                NotifyArtifactCreated(child, result);
                VisitArtifactChildrenAndPublishCreatedEvents(child, result);
            }
        }

        protected void NotifyArtifactCreated(IArtifact artifact, GenerationResult result)
        {
            MessageBus.Publish(new CreatedArtifactEventArgs(result, artifact));
        }

        protected void NotifyArtifactCreating(IArtifact artifact, GenerationResult result)
        {
            MessageBus.Publish(new CreatingArtifactEventArgs(result, artifact));
        }
    }
}
