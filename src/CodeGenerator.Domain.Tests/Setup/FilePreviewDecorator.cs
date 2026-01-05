using CodeGenerator.Core.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Tests
{
    public class FilePreviewDecorator : ArtifactDecorator, IPreviewableDecorator
    {
        private string Content { get; }
        public FilePreviewDecorator(string key, string content) : base(key)
        {
            Content = content;
        }

        public bool CanPreview => true;

        public object? CreatePreview()
        {
            return Content;
        }
    }
}
