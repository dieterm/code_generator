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

            // if not found use the same filename as the existing file

            // determine the target directory
            // visit the parent Artifact until a DirectoryArtifactDecorator is found

            // copy the file
            throw new NotImplementedException();
        }
    }
}
