using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Artifacts.FileSystem
{
    public class File : ArtifactHost
    {
        private readonly FileDecorator _fileDecorator = new FileDecorator();
        public File()
        {
            this.Artifact.AddDecorator(_fileDecorator);
        }

        public File(string fileName) 
            : this()
        {
            this.Artifact.Name = fileName;
        }
    }
}
