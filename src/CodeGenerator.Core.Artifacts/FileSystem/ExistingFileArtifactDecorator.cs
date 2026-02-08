using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class ExistingFileArtifactDecorator : ArtifactDecorator
    {
        public ExistingFileArtifactDecorator(string key, string filePath) : base(key)
        {
            this.FilePath = filePath;
        }

        public ExistingFileArtifactDecorator(ArtifactDecoratorState state) : base(state)
        {

        }

        public string FilePath { 
            get{ return GetValue<string>(nameof(FilePath)); } 
            set{ SetValue<string>(nameof(FilePath), value); }
        }

        public override bool CanGenerate()
        {
            // check if file exists
            return !string.IsNullOrWhiteSpace(FilePath) && System.IO.File.Exists(FilePath);
        }
        public override Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
        {
            // to deterime the target filename
            // first look for a FileArtifactDectorator
            var fileArtifact = this.Artifact as FileArtifact;

            if (fileArtifact == null)
                throw new InvalidOperationException("Artifact of an ExistingFileArtifact should be of type FileArtifact");

            OnGenerating();
            if (string.Equals(FilePath, fileArtifact.FullPath, StringComparison.OrdinalIgnoreCase))
            {
                // If source and target are the same, the file is already at it's place, so we can skip the copy operation.
                return Task.CompletedTask;
            }
            // use the filename from the FileArtifactDecorator
            File.Copy(FilePath, fileArtifact.FullPath, true);
            OnGenerated();
            return Task.CompletedTask;
            
        }
    }
}
