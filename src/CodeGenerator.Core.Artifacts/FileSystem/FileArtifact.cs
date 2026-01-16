using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class FileArtifact : Artifact
    {
        public const string FILE_PROPERTIES_DECORATOR_KEY = "FileProperties";
        public const string TEXT_CONTENT_DECORATOR_KEY = "TextContent";
        private FileArtifactDecorator _fileArtifactDecorator;
        public FileArtifact(string fileName)
        {
            if(string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            _fileArtifactDecorator = AddOrGetDecorator(() => new FileArtifactDecorator(FileArtifact.FILE_PROPERTIES_DECORATOR_KEY));
            ParentChanged += ParentChanged_Handler;
            FileName = fileName;
        }

        public FileArtifact(ArtifactState state) : base(state)
        {
            _fileArtifactDecorator = GetDecoratorOfType<FileArtifactDecorator>() 
                ?? throw new InvalidOperationException($"FileArtifact must have a {nameof(FileArtifactDecorator)} with key '{FILE_PROPERTIES_DECORATOR_KEY}'");
            
            ParentChanged += ParentChanged_Handler;
        }

        private void FileArtifactDecorator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileArtifactDecorator.FileName))
            {
                RaisePropertyChangedEvent(nameof(FileName));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
                RaisePropertyChangedEvent(nameof(FullPath));
            }
        }

        private void ParentChanged_Handler(object? sender, ParentChangedEventArgs e)
        {
            RaisePropertyChangedEvent(nameof(FullPath));
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

        public override T AddDecorator<T>(T decorator)
        {
            if(decorator is FileArtifactDecorator fileDecorator)
            {
                if (_fileArtifactDecorator != null && _fileArtifactDecorator != fileDecorator)
                {
                    throw new InvalidOperationException("FileArtifact can only have one FileArtifactDecorator.");
                }
                if(_fileArtifactDecorator == null)
                    _fileArtifactDecorator = fileDecorator;
                _fileArtifactDecorator.PropertyChanged += FileArtifactDecorator_PropertyChanged;
            }
            return base.AddDecorator(decorator);
        }

        public override void RemoveDecorator(IArtifactDecorator decorator)
        {
            if(decorator is FileArtifactDecorator)
            {
                throw new InvalidOperationException("Cannot remove the FileArtifactDecorator from a FileArtifact.");
            }
            base.RemoveDecorator(decorator);
        }

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
        public string? GetTextContent()
        {
            var textDecorator = GetDecoratorOfType<TextContentDecorator>();
            if(textDecorator!=null) return textDecorator.Content;

            var existingFileDecorator = GetDecoratorOfType<ExistingFileArtifactDecorator>();
            if(existingFileDecorator!=null && !string.IsNullOrWhiteSpace(existingFileDecorator.FilePath))
            {
                return System.IO.File.ReadAllText(existingFileDecorator.FilePath);
            }
            return null;
        }
    }
}
