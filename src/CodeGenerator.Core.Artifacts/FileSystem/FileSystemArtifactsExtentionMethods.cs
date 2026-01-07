using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public static class FileSystemArtifactsExtentionMethods
    {
        public static FileArtifact BecomeFile<TArtifact>(this TArtifact artifact, string fileName)
            where TArtifact : Artifact
        {
            artifact.AddOrGetDecorator(() => new FileArtifactDecorator(FileArtifact.FILE_PROPERTIES_DECORATOR_KEY));
            var newFile = new FileArtifact(artifact);
            newFile.FileName = fileName;
            return newFile;
        }
        public static ExistingFileArtifact BecomeExistingFile<TArtifact>(this TArtifact artifact, string existingFilePath, string? fileName = null)
            where TArtifact : Artifact
        {
            artifact.AddOrGetDecorator(() => new FileArtifactDecorator(FileArtifact.FILE_PROPERTIES_DECORATOR_KEY));
            artifact.AddOrGetDecorator(() => new ExistingFileArtifactDecorator(ExistingFileArtifact.EXISTING_FILE_PROPERTIES_DECORATOR_KEY, existingFilePath));
            var existingFile = new ExistingFileArtifact(artifact);
            existingFile.ExistingFilePath = existingFilePath;
            if (fileName != null)
            {
                existingFile.FileName = fileName;
            }
            else
            {
                existingFile.FileName = System.IO.Path.GetFileName(existingFilePath);
            }
            return existingFile;
        }
    }
}
