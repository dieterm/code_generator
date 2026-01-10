using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class TextContentDecorator : ArtifactDecorator
    {
        public TextContentDecorator(string key) 
            : base(key)
        {

        }

        public string? Content
        {
            get { return GetProperty<string>(nameof(Content)); }
            set { SetProperty(nameof(Content), value); }
        }

        override public bool CanGenerate()
        {
            return !string.IsNullOrWhiteSpace(Artifact.GetDecorator<FileArtifactDecorator>()?.FileName);
        }
        override public async Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
        {
            var fileArtifact = Artifact.GetDecorator<FileArtifactDecorator>() ?? throw new InvalidOperationException("Artifact does not have a FileArtifactDecorator");
            if(string.IsNullOrWhiteSpace(fileArtifact.FileName))
                throw new InvalidOperationException("FileArtifactDecorator does not have a FileName set");

            var folderPath = Artifact.GetFullPath();
            
            var filePath = Path.Combine(folderPath, fileArtifact.FileName);
            var content = Content ?? string.Empty;
            await System.IO.File.WriteAllTextAsync(filePath, content, cancellationToken);
        }
    }
}
