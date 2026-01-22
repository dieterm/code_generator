using CodeGenerator.Shared;
using CodeGenerator.Shared.Views.TreeNode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class ExistingFolderArtifact : FolderArtifact, IEditableTreeNode
    {
        public const string EXISTING_FOLDER_PROPERTIES_DECORATOR_KEY = "ExistingFolderProperties";
        private ExistingFolderArtifactDecorator _existingFolderArtifactDecorator;
        public ExistingFolderArtifact(string existingFolderPath, string? folderName = null)
            : base(folderName ?? System.IO.Path.GetDirectoryName(existingFolderPath))
        {
            if (!Directory.Exists(existingFolderPath)) throw new ArgumentException("The specified existing folder path does not exist.", nameof(existingFolderPath));

            _existingFolderArtifactDecorator = AddOrGetDecorator(() => new ExistingFolderArtifactDecorator(ExistingFolderArtifact.EXISTING_FOLDER_PROPERTIES_DECORATOR_KEY));
            ExistingFolderPath = existingFolderPath;
        }

        public ExistingFolderArtifact(ArtifactState state)
            : base(state)
        {
            _existingFolderArtifactDecorator = GetDecoratorOfType<ExistingFolderArtifactDecorator>()
                ?? throw new InvalidOperationException($"ExistingFolderArtifact must have a {nameof(ExistingFolderArtifactDecorator)} with key '{EXISTING_FOLDER_PROPERTIES_DECORATOR_KEY}'");
        }
        public string ExistingFolderPath
        {
            get { return _existingFolderArtifactDecorator.FolderPath; }
            private set { _existingFolderArtifactDecorator.FolderPath = value; }
        }

        public override T AddDecorator<T>(T decorator)
        {
            if (decorator is ExistingFolderArtifactDecorator existingFolderDecorator)
            {
                if (_existingFolderArtifactDecorator != null && _existingFolderArtifactDecorator != existingFolderDecorator)
                {
                    throw new InvalidOperationException("ExistingFolderArtifact can only have one ExistingFolderArtifactDecorator.");
                }
                if (_existingFolderArtifactDecorator == null)
                    _existingFolderArtifactDecorator = existingFolderDecorator;
                _existingFolderArtifactDecorator.PropertyChanged += ExistingFolderArtifactDecorator_PropertyChanged;
            }
            return base.AddDecorator(decorator);
        }

        public override void RemoveDecorator(IArtifactDecorator decorator)
        {
            if (decorator is ExistingFolderArtifactDecorator)
            {
                throw new InvalidOperationException("Cannot remove the ExistingFolderArtifactDecorator from an ExistingFolderArtifact.");
            }
            base.RemoveDecorator(decorator);
        }

        private void ExistingFolderArtifactDecorator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExistingFolderArtifactDecorator.FolderPath))
            {
                RaisePropertyChangedEvent(nameof(ExistingFolderPath));
            }
        }

        public bool CanBeginEdit()
        {
            return Directory.Exists(ExistingFolderPath);
        }

        public bool Validating(string newName)
        {
            if (newName == this.FolderName)
                return false;
            var parentDirectory = System.IO.Path.GetDirectoryName(ExistingFolderPath);
            var newFolder = System.IO.Path.Combine(parentDirectory ?? string.Empty, newName);
            return !Directory.Exists(newFolder);
        }

        public void EndEdit(string oldName, string newName)
        {
            try
            {
                var parentDirectory = System.IO.Path.GetDirectoryName(ExistingFolderPath);
                var newFolder = System.IO.Path.Combine(parentDirectory ?? string.Empty, newName);
                if (Directory.Exists(ExistingFolderPath) && !Directory.Exists(newFolder))
                {
                    Directory.Move(ExistingFolderPath, newFolder);
                    ExistingFolderPath = newFolder;
                    this.FolderName = newName;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
