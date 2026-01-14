using CodeGenerator.Core.Templates;

namespace CodeGenerator.TemplateEngines.T4
{
    public class T4TemplateInstance : ITemplateInstance
    {
        private readonly T4Template _template;

        public T4TemplateInstance(T4Template template)
        {
            _template = template;
        }

        public ITemplate Template => _template;

        /// <summary>
        /// Parameters to pass to the T4 template (accessible via Host.ResolveParameterValue)
        /// </summary>
        public Dictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>();

        /// <summary>
        /// Output file name for the generated content
        /// </summary>
        public string? OutputFileName { get; set; }

        /// <summary>
        /// Additional assembly references required by the template
        /// </summary>
        public List<string> AssemblyReferences { get; } = new List<string>();

        /// <summary>
        /// Additional namespace imports for the template
        /// </summary>
        public List<string> Imports { get; } = new List<string>();

        /// <summary>
        /// Additional include directories for template resolution
        /// </summary>
        public List<string> IncludeDirectories { get; } = new List<string>();

        public void SetParameter(string key, object? value)
        {
            Parameters[key] = value;// ?? throw new ArgumentNullException(nameof(value), $"Parameter '{key}' cannot be set to null.");
        }
    }
}