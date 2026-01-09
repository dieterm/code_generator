using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.FileSystem
{
    public class TextContentDecorator : ArtifactDecorator
    {
        public TextContentDecorator(string key) 
            : base(key)
        {

        }

        public string? Content
        {
            get { return GetProperty<string>(nameof(Content)); }
            set { SetProperty(nameof(Content), value); }
        }

    }
}
