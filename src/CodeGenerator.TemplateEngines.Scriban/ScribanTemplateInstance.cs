using CodeGenerator.Core.Templates;

namespace CodeGenerator.TemplateEngines.Scriban
{
    public class ScribanTemplateInstance : ITemplateInstance
    {
        private readonly ScribanTemplate _template;
        public ScribanTemplateInstance(ScribanTemplate template)
        {
            _template = template;
        }
        public ITemplate Template { get { return _template; } }
        /// <summary>
        /// If set, specifies the output file name for this template instance.
        /// If null, the default naming strategy will be used (=template name without .scriban extension)
        /// </summary>
        public string? OutputFileName { get; set; }
        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
        public Dictionary<string, Delegate> Functions { get; } = new Dictionary<string, Delegate>();
        public List<string> ExtraTemplateLocations { get; } = new List<string>();
    }
}