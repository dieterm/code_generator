using CodeGenerator.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    public class TemplateEngineManager
    {
        private readonly ILogger _logger;
        private readonly List<ITemplateEngine> _templateEngines;
        public IEnumerable<ITemplateEngine> TemplateEngines { get { return _templateEngines; } }

        public TemplateEngineManager(IEnumerable<ITemplateEngine> templateEngines, ILogger<TemplateEngineManager> logger)
        {
            _templateEngines = templateEngines?.ToList() ?? throw new ArgumentNullException(nameof(templateEngines));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ITemplateEngine? GetTemplateEngineById(string id)
        {
            var engine = _templateEngines.FirstOrDefault(te => te.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (engine == null)
            {
                _logger.LogWarning("Template engine with ID '{TemplateEngineId}' not found.", id);
            }
            return engine;
        }
       
        /// <summary>
        /// Get a template engine that supports the specified file extension (extension without dot, e.g. "txt", "scriban").
        /// </summary>
        /// <param name="fileExtension">extension without dot, e.g. "txt", "scriban"</param>
        /// <returns></returns>
        public ITemplateEngine? GetTemplateEngineByFileExtension(string fileExtension)
        {
            var engine = _templateEngines.FirstOrDefault(te => te.SupportsTemplateFileExtension(fileExtension));
            if (engine == null)
            {
                //_logger.LogWarning("No template engine found that supports file extension '{FileExtension}'.", fileExtension);
            }
            return engine;
        }

        public ITemplateEngine? GetTemplateEngineForTemplateInstance(ITemplateInstance templateInstance)
        {
            return _templateEngines.FirstOrDefault(te => te.SupportsTemplate(templateInstance.Template));
        }
        public IEnumerable<ITemplateEngine> GetTemplateEnginesForTemplateInstance(ITemplateInstance templateInstance)
        {
            return _templateEngines.Where(te => te.SupportsTemplate(templateInstance.Template)).ToArray();
        }

        public IEnumerable<ITemplateEngine> GetTemplateEnginesForTemplate(ITemplate template)
        {
            return _templateEngines.Where(te => te.SupportsTemplate(template)).ToArray();
        } 

        public async Task<TemplateOutput> RenderTemplateAsync(ITemplateInstance templateInstance, CancellationToken cancellationToken = default)
        {
            var engine = GetTemplateEngineForTemplateInstance(templateInstance);
            if (engine == null)
            {
                var errorMessage = $"No template engine found that supports template '{templateInstance.Template.TemplateId}' of type '{templateInstance.Template.TemplateType}'.";
                _logger.LogError(errorMessage);
                return new TemplateOutput(errorMessage);              
            }
            _logger.LogInformation("Rendering template '{TemplateName}'({TemplateType}) using engine '{TemplateEngineId}'.", templateInstance.Template.TemplateId, templateInstance.Template.TemplateType, engine.Id);
            return await engine.RenderAsync(templateInstance, cancellationToken);
        }

    }
}
