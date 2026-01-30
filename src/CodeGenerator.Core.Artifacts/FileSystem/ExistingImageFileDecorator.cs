using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Shared.ExtensionMethods;
namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class ExistingImageFileDecorator : ArtifactDecorator, IPreviewableDecorator
    {
        public ExistingImageFileDecorator(ArtifactDecoratorState state) : base(state)
        {
        }

        public ExistingImageFileDecorator(string key) : base(key)
        {
        }

        public bool CanPreview => Artifact is ExistingFileArtifact existingFileArtifact && existingFileArtifact.ExistingFilePath.IsImageFile();

        public object? CreatePreview()
        {
            var existingFileArtifact = Artifact as ExistingFileArtifact;
            if (existingFileArtifact != null && existingFileArtifact.ExistingFilePath.IsImageFile())
            {
                return Image.FromFile(existingFileArtifact.ExistingFilePath);
            }
            return null;
        }
    }
}
