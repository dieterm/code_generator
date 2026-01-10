using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class ExistingFolderArtifact : FolderArtifact
    {
        public const string EXISTING_FOLDER_PROPERTIES_DECORATOR_KEY = "ExistingFolderProperties";
        private readonly ExistingFolderArtifactDecorator _existingFolderArtifactDecorator;
        public ExistingFolderArtifact(string existingFolderPath, string? folderName = null) : base(folderName ?? System.IO.Path.GetDirectoryName(existingFolderPath))
        {
            if (!Directory.Exists(existingFolderPath)) throw new ArgumentException("The specified existing folder path does not exist.", nameof(existingFolderPath));

            _existingFolderArtifactDecorator = AddOrGetDecorator(() => new ExistingFolderArtifactDecorator(ExistingFolderArtifact.EXISTING_FOLDER_PROPERTIES_DECORATOR_KEY));
            
            ExistingFolderPath = existingFolderPath;
        }

        public string ExistingFolderPath
        {
            get { return _existingFolderArtifactDecorator.FolderPath; }
            private set { _existingFolderArtifactDecorator.FolderPath = value; }
        }
    }
}
