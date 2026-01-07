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


        public ExistingFileArtifact(Artifact artifact, string existingFilePath, string? fileName = null) : base (artifact, fileName?? System.IO.Path.GetFileName(existingFilePath))
        {
            if(!File.Exists(existingFilePath)) throw new ArgumentException("The specified existing file path does not exist.", nameof(existingFilePath));

            artifact.AddOrGetDecorator(() => new FileArtifactDecorator(FileArtifact.FILE_PROPERTIES_DECORATOR_KEY));
            artifact.AddOrGetDecorator(() => new ExistingFileArtifactDecorator(ExistingFileArtifact.EXISTING_FILE_PROPERTIES_DECORATOR_KEY, existingFilePath));

            ExistingFilePath = existingFilePath;
        }

        public string? ExistingFilePath
        {
            get { return Artifact.GetDecorator<ExistingFileArtifactDecorator>()?.FilePath; }
            private set { Artifact.GetDecorator<ExistingFileArtifactDecorator>().FilePath = value; }
        }
    }
}
