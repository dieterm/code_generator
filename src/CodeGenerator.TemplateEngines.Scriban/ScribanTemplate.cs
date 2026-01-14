using CodeGenerator.Core.Templates;

namespace CodeGenerator.TemplateEngines.Scriban
{
    public abstract class ScribanTemplate : ITemplate
    {
        public ScribanTemplate(string templateId, bool useCaching = false)
        {
            TemplateId = templateId;
            UseCaching = useCaching;
        }

        public abstract string Content { get; }

        public TemplateType TemplateType { get { return TemplateType.Scriban; } }

        public string TemplateId { get; }

        public bool UseCaching { get; set; } = false;
    }
}