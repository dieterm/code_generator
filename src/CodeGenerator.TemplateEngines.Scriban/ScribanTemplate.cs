using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.Views.TreeNode;

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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ITreeNodeIcon Icon { get; } = new AssemblyResourceTreeNodeIcon(typeof(ScribanTemplate).Assembly, "CodeGenerator.TemplateEngines.Scriban.scriban-icon.png");

    }
}