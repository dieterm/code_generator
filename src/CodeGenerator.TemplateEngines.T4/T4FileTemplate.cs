using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.T4
{
    public class T4FileTemplate : T4Template
    {
        private readonly Lazy<string> _content;
        public string FilePath { get; }
        public T4FileTemplate(string templateId, string filePath)
            : base(templateId)
        {
            FilePath = filePath;
            _content = new Lazy<string>(() => System.IO.File.ReadAllText(filePath));
        }

        public override string Content { get { return _content.Value; } }
    }
}
