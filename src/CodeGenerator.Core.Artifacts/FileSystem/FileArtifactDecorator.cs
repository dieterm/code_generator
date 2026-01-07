using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class FileArtifactDecorator : ArtifactDecorator
    {
        public FileArtifactDecorator(string key) : base(key)
        {
        }

        /// <summary>
        /// Name of the file with extension (eg. ".gitignore', 'MyClass.cs', ...)
        /// </summary>
        public string FileName {
            get { return GetProperty<string>(nameof(FileName)); }
            set { SetProperty(nameof(FileName), value); }
        }
    }
}
