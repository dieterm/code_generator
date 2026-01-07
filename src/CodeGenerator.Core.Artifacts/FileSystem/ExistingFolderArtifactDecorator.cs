using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class ExistingFolderArtifactDecorator : ArtifactDecorator
    {
        public ExistingFolderArtifactDecorator(string key, string folderPath) : base(key)
        {
            this.FolderPath = folderPath;
        }

        public string FolderPath
        {
            get { return GetProperty<string>(nameof(FolderPath)); }
            set { SetProperty(nameof(FolderPath), value); }
        }

        public override bool CanGenerate()
        {
            // check if folder exists
            return !string.IsNullOrWhiteSpace(FolderPath) && System.IO.Directory.Exists(FolderPath);
        }

        public override Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
        {
            // to determine the target foldername
            // first look for a FolderArtifactDecorator

            // if not found use the same foldername as the existing folder

            // determine the target directory
            // visit the parent Artifact until a DirectoryArtifactDecorator is found

            // copy the folder
            throw new NotImplementedException();
        }
    }
}
