using CodeGenerator.Core.Templates;

namespace CodeGenerator.Core.Generators.Settings
{
    public class TemplateRequirement
    {
        public TemplateRequirement(string templateId, string description, string outputFileNamePattern, TemplateType templateType)
        {
            TemplateId = templateId;
            Description = description;
            OutputFileNamePattern = outputFileNamePattern;
            TemplateType = templateType;
        }
        /// <summary>
        /// Id of the template as to be defined in settings
        /// </summary>
        public string TemplateId { get; }
        /// <summary>
        /// Description of what the template does
        /// </summary>
        public string Description { get; } 
        /// <summary>
        /// Type of template (eg. Scriban, T4, TextFile,...)
        /// </summary>
        public TemplateType TemplateType { get; }
        /// <summary>
        /// Output file name pattern (e.g. '{{EntityName}}Controller.cs')
        /// </summary>
        public string OutputFileNamePattern { get; }
    }
}
