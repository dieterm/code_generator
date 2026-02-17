using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class ExistingFolderArtifactDecorator : ArtifactDecorator
    {
        public ExistingFolderArtifactDecorator(string key) : base(key)
        {

        }
        public ExistingFolderArtifactDecorator(ArtifactDecoratorState state, List<string> errors) : base(state, errors)
        {

        }

        public string FolderPath
        {
            get { return GetValue<string>(nameof(FolderPath)); }
            set { SetValue<string>(nameof(FolderPath), value); }
        }

    }
}
