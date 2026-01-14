using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using System.ComponentModel;
namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class FolderArtifact : Artifact
    {
        public const string FOLDER_PROPERTIES_DECORATOR_KEY = "FolderProperties";
        private FolderArtifactDecorator _folderArtifactDecorator;
        public FolderArtifact(string folderName)
        {
            if(string.IsNullOrWhiteSpace(folderName)) throw new ArgumentNullException(nameof(folderName));
            _folderArtifactDecorator = AddOrGetDecorator(() => new FolderArtifactDecorator(FolderArtifact.FOLDER_PROPERTIES_DECORATOR_KEY));
            FolderName = folderName;
        }

        public FolderArtifact(ArtifactState state)
            : base(state)
        {
            _folderArtifactDecorator = GetDecorator<FolderArtifactDecorator>()
                ?? throw new InvalidOperationException($"FolderArtifact must have a {nameof(FolderArtifactDecorator)} with key '{FOLDER_PROPERTIES_DECORATOR_KEY}'");
        }

        override public string TreeNodeText { get { return FolderName; } }
        override public ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("folder");
        public string FolderName {
            get { return GetDecorator<FolderArtifactDecorator>().FolderName; }
            set { GetDecorator<FolderArtifactDecorator>().FolderName = value; }
        }
        public string FullPath { get { return this.GetFullPath(); } }

        public override T AddDecorator<T>(T decorator)
        {
            if(decorator is FolderArtifactDecorator folderDecorator)
            {
                if (_folderArtifactDecorator != null && _folderArtifactDecorator != folderDecorator)
                {
                    throw new InvalidOperationException("FolderArtifact can only have one FolderArtifactDecorator.");
                }
                if (_folderArtifactDecorator == null)
                    _folderArtifactDecorator = folderDecorator;
                _folderArtifactDecorator.PropertyChanged += FolderArtifactDecorator_PropertyChanged;
            }
            return base.AddDecorator(decorator);
        }

        private void FolderArtifactDecorator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FolderArtifactDecorator.FolderName))
            {
                RaisePropertyChangedEvent(nameof(FolderName));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
                RaisePropertyChangedEvent(nameof(FullPath));
            }
        }
    }
}
