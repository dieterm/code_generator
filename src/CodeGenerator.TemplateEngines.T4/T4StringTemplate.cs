using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.T4
{
    public class T4StringTemplate : T4Template
    {
        public T4StringTemplate(string content)
            : this(Guid.NewGuid().ToString(), content)
        {
        }

        public T4StringTemplate(string templateId, string content) 
            : base(templateId)
        {
            if(string.IsNullOrWhiteSpace(content)) throw new ArgumentNullException(nameof(content));
            Content = content;
        }

        public override string Content { get; }
    }
}
