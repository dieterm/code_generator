using CodeGenerator.Domain.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Tests
{
    public class FileArtifact : Artifact
    {
        private FileDecorator FileDecorator { get { return Decorators[nameof(FileDecorator)] as FileDecorator; } }
        private FilePreviewDecorator FilePreviewDecorator { get { return Decorators[nameof(FilePreviewDecorator)] as FilePreviewDecorator; } }

        public FileArtifact() {
            AddDecorator(new FileDecorator(nameof(FileDecorator)));
            AddDecorator(new FilePreviewDecorator(nameof(FilePreviewDecorator), "Hello world"));
        }

        public string FileName
        {
            get { return FileDecorator.FileName; }
            set { FileDecorator.FileName = value; }
        }
        public string Extension
        {
            get { return FileDecorator.Extension; }
            set { FileDecorator.Extension = value; }
        }
        public long Size
        {
            get { return FileDecorator.Size; }
            set { FileDecorator.Size = value; }
        }
    }
}
