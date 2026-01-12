using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class FolderArtifactDecorator : ArtifactDecorator
    {
        public FolderArtifactDecorator(string key) : base(key)
        {

        }

        /// <summary>
        /// Name of the folder (eg. "src", "bin", ...)
        /// </summary>
        public string? FolderName
        {
            get { return GetValue<string>(nameof(FolderName)); }
            set { SetValue<string>(nameof(FolderName), value); }
        }

        public override bool CanGenerate()
        {
            return !string.IsNullOrWhiteSpace(FolderName);
        }

        public override Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
        {
            var newFolderPath = Path.Combine(this.Artifact.GetFullPath());
            System.IO.Directory.CreateDirectory(newFolderPath);
            return Task.CompletedTask;
        }
    }
}
