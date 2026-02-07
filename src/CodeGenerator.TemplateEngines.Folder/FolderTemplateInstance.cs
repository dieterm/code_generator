using CodeGenerator.Core.Templates;
using CodeGenerator.Shared;
using CodeGenerator.TemplateEngines.Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.Folder
{
    public class FolderTemplateInstance : ITemplateInstance
    {
        public ITemplate Template { get; }

        public FolderTemplateInstance(FolderTemplate template)
        {
            Template = template;
        }
        /// <summary>
        /// This is the common template handler that will be used for all templates inside the folder
        /// </summary>
        public TemplateHandler? TemplateHandler { get; set; }
        /// <summary>
        /// Shared parameters for all templates inside the folder. 
        /// This can be used to pass common data to all child templates when rendering the folder as a whole.
        /// </summary>
        public Dictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>();
        public List<TemplateHandler> TemplateHandlers { get; } = new List<TemplateHandler>();

        public List<string> ExcludedExtensions { get; } = new List<string> { TemplateDefinition.DefinitionFileExtension.Replace(".", "") };
        public List<string> ExcludedFileNames { get; } = new List<string>();
        /// <summary>
        /// Filter out folders by their name (without relative path)
        /// </summary>
        public List<string> ExcludedFolderNames { get; } = new List<string> { "obj", "bin" };
        /// <summary>
        /// Filter out folders by also specifying their relative path
        /// eg. "subfolder/excludeFolderName"
        /// </summary>
        public List<string> ExcludedFolders { get; } = new List<string> { "obj", "bin" };

        public void SetParameter(string key, object? value)
        {
            Parameters[key] = value;
        }

        public Task<TemplateOutput> RenderAsync(CancellationToken cancellationToken)
        {
            var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
            var folderTemplateEngine = templateEngineManager.GetTemplateEnginesForTemplate(Template)
                .OfType<FolderTemplateEngine>()
                .FirstOrDefault();
            if (folderTemplateEngine == null)
                throw new ApplicationException($"No Folder template engine found for template id '{Template.TemplateId}'.");
            return folderTemplateEngine.RenderAsync(this, cancellationToken);
        }
    }
}
