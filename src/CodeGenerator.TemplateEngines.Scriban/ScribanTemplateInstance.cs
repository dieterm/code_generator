using CodeGenerator.Core.Templates;
using CodeGenerator.Shared;

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
        public Dictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>();
        public Dictionary<string, Delegate> Functions { get; } = new Dictionary<string, Delegate>();
        public List<string> ExtraTemplateLocations { get; } = new List<string>();

        public void SetParameter(string key, object? value)
        {
            if(key == nameof(OutputFileName))
            {
                if(value is string outputFileName)
                {
                    OutputFileName = outputFileName;
                    return;
                }
                else
                {
                    throw new ArgumentException($"Value for parameter '{key}' must be of type string or null.", nameof(value));
                }
            }
            Parameters[key] = value;
        }

        public Task<TemplateOutput> RenderAsync(CancellationToken cancellationToken)
        {
            var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
            var scribanTemplateEngine = templateEngineManager.GetTemplateEnginesForTemplate(_template)
                .OfType<ScribanTemplateEngine>()
                .FirstOrDefault();
            if (scribanTemplateEngine == null)
                throw new ApplicationException($"No Scriban template engine found for template id '{_template.TemplateId}'.");
            return scribanTemplateEngine.RenderAsync(this, cancellationToken);
        }
    }
}