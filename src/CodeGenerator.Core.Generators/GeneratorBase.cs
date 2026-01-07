using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
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
        public GeneratorSettingsDescription SettingsDescription { get; }

        protected GeneratorBase()
        {
            SettingsDescription = ConfigureSettingsDescription();
        }

        protected abstract GeneratorSettingsDescription ConfigureSettingsDescription();

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
        }
    }
}
