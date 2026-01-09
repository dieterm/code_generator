using CodeGenerator.Core.Templates;

namespace CodeGenerator.TemplateEngines.Scriban
{
    public abstract class ScribanTemplate : ITemplate
    {
        public ScribanTemplate(string templateId)
        {
            TemplateId = templateId;
        }

        public abstract string Content { get; }

        public TemplateType TemplateType { get { return TemplateType.Scriban; } }

        public string TemplateId { get; }
    }
}