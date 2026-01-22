using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Tests
{
    public class GeneratorA
    {
        private readonly TestMessageBus _messageBus;
        public GeneratorA(TestMessageBus messageBus)
        {
            _messageBus = messageBus;
            _messageBus.Subscribe<ArtifactCreating>(OnArtifactCreating);
        }

        private void OnArtifactCreating(ArtifactCreating e)
        {
            // Handle the event
        }

        public FileArtifact PublishEvent()
        {
            var artifact = new FileArtifact { Name = "TestArtifact", Extension = ".txt", Size = 1024 };
            _messageBus.Publish(new ArtifactCreating(artifact));
            return artifact;
        }
    }
}
