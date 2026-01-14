using CodeGenerator.Core.Templates;

namespace CodeGenerator.TemplateEngines.T4
{
    public abstract class T4Template : ITemplate
    {
        public T4Template(string id)
        {
            TemplateId = id;
        }
        public string TemplateId { get; }
        public TemplateType TemplateType { get { return TemplateType.T4; } }

        public abstract string Content { get; }

        public bool UseCaching { get; set; } = false;
    }
}