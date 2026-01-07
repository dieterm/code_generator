using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public static class FileSystemExtentionMethods
    {
        public static FolderArtifact GetParentFolderArtifact(this IArtifact artifact)
        {
            IArtifact? current = artifact.Parent;
            while (current != null)
            {
                if (current is FolderArtifact folderArtifact || current.GetDecorator<FolderArtifactDecorator>() != null)
                {
                    return (FolderArtifact)current;
                }
                current = current.Parent;
            }
            //throw new InvalidOperationException("The artifact does not have a parent folder.");
            return null;
        }

        public static bool IsFileArtifact(this IArtifact artifact)
        {
            return artifact.GetDecorator<FileArtifactDecorator>() != null;
        }
        public static bool IsExistingFileArtifact(this IArtifact artifact)
        {
            return IsFileArtifact(artifact) && artifact.GetDecorator<ExistingFileArtifactDecorator>() != null;
        }
        public static bool IsFolderArtifact(this IArtifact artifact)
        {
            return artifact.GetDecorator<FolderArtifactDecorator>() != null;
        }
        public static bool IsExistingFolderArtifact(this IArtifact artifact)
        {
            return IsFolderArtifact(artifact) && artifact.GetDecorator<ExistingFolderArtifactDecorator>() != null;
        }
    }
}
