using CodeGenerator.Core.Artifacts.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public static class FileSystemExtentionMethods
    {
        public static string? GetFullPath(this IArtifact artifact)
        {
            var folderParts = artifact.GetAllFolderParts();
            folderParts.Reverse();
            var finalFolderParts = folderParts.ToArray();
            return System.IO.Path.Combine(finalFolderParts);
        }
        // decendant search for FolderArtifactDecorator
        public static List<string> GetAllFolderParts(this IArtifact artifact, List<string>? path = null)
        {
            if(path == null)
            {
                path = new List<string>();
            }
            var folderDecorator = artifact.GetDecoratorOfType<FolderArtifactDecorator>();
            if (folderDecorator != null)
            {
                path.Add(folderDecorator.FolderName);
            }
            if (artifact.Parent != null)
            {
                if(artifact.Parent is RootArtifact rootArtifact)
                {
                    path.Add(rootArtifact.FolderPath);
                } else { 
                    artifact.Parent.GetAllFolderParts(path);
                }
            }
            return path;
        }
        public static FolderArtifactDecorator? GetFirstFolderArtifactDecoratorUpward(this IArtifact artifact)
        {
            IArtifact? current = artifact;
            while (current != null)
            {
                var folderDecorator = current.GetDecoratorOfType<FolderArtifactDecorator>();
                if (folderDecorator != null)
                {
                    return folderDecorator;
                }
                current = current.Parent;
            }
            return null;
        }
        public static FolderArtifact GetParentFolderArtifact(this IArtifact artifact)
        {
            IArtifact? current = artifact.Parent;
            while (current != null)
            {
                if (current is FolderArtifact folderArtifact || current.GetDecoratorOfType<FolderArtifactDecorator>() != null)
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
            return artifact.GetDecoratorOfType<FileArtifactDecorator>() != null;
        }
        public static bool IsExistingFileArtifact(this IArtifact artifact)
        {
            return IsFileArtifact(artifact) && artifact.GetDecoratorOfType<ExistingFileArtifactDecorator>() != null;
        }
        public static bool IsFolderArtifact(this IArtifact artifact)
        {
            return artifact.GetDecoratorOfType<FolderArtifactDecorator>() != null;
        }
        public static bool IsExistingFolderArtifact(this IArtifact artifact)
        {
            return IsFolderArtifact(artifact) && artifact.GetDecoratorOfType<ExistingFolderArtifactDecorator>() != null;
        }
    }
}
