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

        public FileArtifactDecorator(ArtifactDecoratorState state, List<string> errors) : base(state, errors)
        {
            
        }

        /// <summary>
        /// Name of the file with extension (eg. ".gitignore', 'MyClass.cs', ...)
        /// </summary>
        public string FileName {
            get { return GetValue<string>(nameof(FileName)); }
            set { SetValue(nameof(FileName), value); }
        }
    }
}
