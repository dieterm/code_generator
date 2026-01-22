using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.Scriban
{
    public class ScribanStringTemplate : ScribanTemplate
    {
        public ScribanStringTemplate(string content, bool useCaching = false)
            : this(Guid.NewGuid().ToString(), content, useCaching)
        {
        }

        public ScribanStringTemplate(string templateId, string content, bool useCaching = false) 
            : base(templateId, useCaching)
        {
            if(string.IsNullOrWhiteSpace(content)) throw new ArgumentNullException(nameof(content));
            Content = content;
        }

        public override string Content { get; }
    }
}
