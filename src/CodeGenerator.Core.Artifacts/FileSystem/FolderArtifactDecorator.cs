using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class FolderArtifactDecorator : ArtifactDecorator
    {
        public FolderArtifactDecorator(string key) : base(key)
        {

        }

        /// <summary>
        /// Name of the folder (eg. "src", "bin", ...)
        /// </summary>
        public string? FolderName
        {
            get { return GetProperty<string>(nameof(FolderName)); }
            set { SetProperty(nameof(FolderName), value); }
        }
    }
}
