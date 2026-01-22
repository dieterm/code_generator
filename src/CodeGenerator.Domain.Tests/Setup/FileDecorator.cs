using CodeGenerator.Core.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Tests
{
    public class FileDecorator : ArtifactDecorator
    {
        public FileDecorator(string key)
            : base(key)
        {
            
        }

        public string FileName { 
            get { return GetProperty<string>(nameof(FileName)) ?? string.Empty; }
            set { SetProperty(nameof(FileName), value); } 
        }
        public string Extension { 
            get { return GetProperty<string>(nameof(Extension)) ?? string.Empty; } 
            set { SetProperty(nameof(Extension), value); } 
        }
        public long Size { 
            get { return GetProperty<long>(nameof(Size)); } 
            set { SetProperty(nameof(Size), value); }
        }
    }
}
