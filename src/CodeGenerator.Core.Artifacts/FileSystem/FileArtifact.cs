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
        public const string TEXT_CONTENT_DECORATOR_KEY = "TextContent";
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

        public void SetTextContent(string content)
        {
            AddOrGetDecorator<TextContentDecorator>(() => new TextContentDecorator(TEXT_CONTENT_DECORATOR_KEY))
                .Content = content;
        }

        /// <summary>
        /// Try to get the TextContentDecorator or ExistingFileArtifactDecorator of the file artifact.<br/>
        /// If the TextContentDecorator is set, its content is returned.<br/>
        /// Otherwise, if the ExistingFileArtifactDecorator is set and has a valid file path, the content of the existing file is read and returned.<br/>
        /// If neither decorator is set, null is returned.
        /// </summary>
        public string? GetTextContext()
        {
            var textDecorator = GetDecorator<TextContentDecorator>();
            if(textDecorator!=null) return textDecorator.Content;

            var existingFileDecorator = GetDecorator<ExistingFileArtifactDecorator>();
            if(existingFileDecorator!=null && !string.IsNullOrWhiteSpace(existingFileDecorator.FilePath))
            {
                return System.IO.File.ReadAllText(existingFileDecorator.FilePath);
            }
            return null;
        }
    }
}
