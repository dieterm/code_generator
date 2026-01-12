using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class ExistingFileArtifact : FileArtifact
    {
        public const string EXISTING_FILE_PROPERTIES_DECORATOR_KEY = "ExistingFile";
        private readonly ExistingFileArtifactDecorator _existingFileArtifactDecorator;
        public ExistingFileArtifact(string existingFilePath, string? fileName = null) 
            : base (fileName?? System.IO.Path.GetFileName(existingFilePath))
        {
            if(!File.Exists(existingFilePath)) throw new ArgumentException("The specified existing file path does not exist.", nameof(existingFilePath));
            _existingFileArtifactDecorator = AddOrGetDecorator(() => new ExistingFileArtifactDecorator(ExistingFileArtifact.EXISTING_FILE_PROPERTIES_DECORATOR_KEY, existingFilePath));
            
            ExistingFilePath = existingFilePath;
        }

        public ExistingFileArtifact(ArtifactState state)
            : base(state)
        {
            _existingFileArtifactDecorator = GetDecorator<ExistingFileArtifactDecorator>()
                ?? throw new InvalidOperationException($"ExistingFileArtifact must have a {nameof(ExistingFileArtifactDecorator)} with key '{EXISTING_FILE_PROPERTIES_DECORATOR_KEY}'");
        }

        public string ExistingFilePath
        {
            get { return _existingFileArtifactDecorator.FilePath; }
            private set { _existingFileArtifactDecorator.FilePath = value; }
        }

        public override void RemoveDecorator(IArtifactDecorator decorator)
        {
            if (decorator is ExistingFileArtifactDecorator)
            {
                throw new InvalidOperationException("Cannot remove the ExistingFileArtifactDecorator from an ExistingFileArtifact.");
            }
            base.RemoveDecorator(decorator);
        }

        public override T AddDecorator<T>(T decorator)
        {
            if (decorator is ExistingFileArtifactDecorator existingFileDecorator)
            {
                if (_existingFileArtifactDecorator != null && _existingFileArtifactDecorator != existingFileDecorator)
                {
                    throw new InvalidOperationException("ExistingFileArtifact can only have one ExistingFileArtifactDecorator.");
                }
                _existingFileArtifactDecorator.PropertyChanged += ExistingFileArtifactDecorator_PropertyChanged;
            }
            return base.AddDecorator(decorator);
        }

        private void ExistingFileArtifactDecorator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExistingFileArtifactDecorator.FilePath))
            {
                RaisePropertyChangedEvent(nameof(ExistingFilePath));
            }
        }
    }
}
