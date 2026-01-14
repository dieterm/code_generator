using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.Scriban
{
    public class ScribanFileTemplate : ScribanTemplate
    {
        private readonly Lazy<string> _content;
        public string FilePath { get; }
        public bool CreateTemplateFileIfMissing { get; set; }
        public ScribanFileTemplate(string templateId, string filePath, bool useCaching = false) 
            : base(templateId, useCaching)
        {
            FilePath = filePath;
            _content = new Lazy<string>(() => System.IO.File.ReadAllText(filePath));
        }

        public override string Content { get { return _content.Value; } }
    }
}
