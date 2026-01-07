using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class FileArtifact : Artifact
    {
        public const string FILE_PROPERTIES_DECORATOR_KEY = "FileProperties";
        private readonly FileArtifactDecorator _fileArtifactDecorator;
        public FileArtifact(string fileName)
        {
            if(string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            _fileArtifactDecorator = AddOrGetDecorator(() => new FileArtifactDecorator(FileArtifact.FILE_PROPERTIES_DECORATOR_KEY));
            FileName = fileName;
        }

        public string FileName { 
            get{ return _fileArtifactDecorator.FileName; } 
            private set{ _fileArtifactDecorator.FileName = value; } 
        }

        public string FullPath
        {
            get
            {
                var parentFolder = this.GetParentFolderArtifact();
                if (parentFolder == null) return FileName;
                return $"{parentFolder.FullPath}{System.IO.Path.DirectorySeparatorChar}{FileName}";
            }
        }
        public override string TreeNodeText { get { return FileName; } }
        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("file-plus-corner");
        public override string Id => FullPath;
    }
}
