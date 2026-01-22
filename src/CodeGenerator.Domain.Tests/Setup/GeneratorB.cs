using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Tests
{
    public class GeneratorB
    {
        private readonly TestMessageBus _messageBus;
        public GeneratorB(TestMessageBus messageBus)
        {
            _messageBus = messageBus;
            _messageBus.Subscribe<ArtifactCreating>(OnArtifactCreating, (ac) => ac.Artifact is FileArtifact fileArtifact && fileArtifact.Extension== ".txt");
        }

        private void OnArtifactCreating(ArtifactCreating creating)
        {
            if(creating.Artifact is FileArtifact fileArtifact)
            {
                var filePreviewDecorator = fileArtifact.GetDecorator<FilePreviewDecorator>();
                fileArtifact.RemoveDecorator(filePreviewDecorator);
                var newFilePreviewDecorator = new FilePreviewDecorator(nameof(FilePreviewDecorator),"New preview content from GeneratorB");
                fileArtifact.AddDecorator(newFilePreviewDecorator);
            }
        }
    }
}
