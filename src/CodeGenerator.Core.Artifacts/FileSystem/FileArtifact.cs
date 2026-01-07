using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class FileArtifact : ArtifactHost
    {
        public const string FILE_PROPERTIES_DECORATOR_KEY = "FileProperties";
        
        public FileArtifact(Artifact artifact, string fileName) : base(artifact)
        {
            if(string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            artifact.AddOrGetDecorator(() => new FileArtifactDecorator(FileArtifact.FILE_PROPERTIES_DECORATOR_KEY));
            FileName = fileName;
        }

        public string? FileName { 
            get{ return Artifact.GetDecorator<FileArtifactDecorator>()?.FileName; } 
            private set{ Artifact.GetDecorator<FileArtifactDecorator>().FileName = value; } 
        }

        public override string TreeNodeText { get { return FileName ?? throw new InvalidOperationException("FileName is not set."); } }
    }
}
