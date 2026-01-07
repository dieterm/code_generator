using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class ExistingFileArtifact : FileArtifact
    {
        public const string EXISTING_FILE_PROPERTIES_DECORATOR_KEY = "ExistingFileProperties";
        private readonly ExistingFileArtifactDecorator _existingFileArtifactDecorator;
        public ExistingFileArtifact(string existingFilePath, string? fileName = null) : base (fileName?? System.IO.Path.GetFileName(existingFilePath))
        {
            if(!File.Exists(existingFilePath)) throw new ArgumentException("The specified existing file path does not exist.", nameof(existingFilePath));

            _existingFileArtifactDecorator = AddOrGetDecorator(() => new ExistingFileArtifactDecorator(ExistingFileArtifact.EXISTING_FILE_PROPERTIES_DECORATOR_KEY, existingFilePath));

            ExistingFilePath = existingFilePath;
        }

        public string ExistingFilePath
        {
            get { return _existingFileArtifactDecorator.FilePath; }
            private set { _existingFileArtifactDecorator.FilePath = value; }
        }
    }
}
