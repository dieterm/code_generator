using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.Scriban
{
    public class DefaultTemplateLoader : ITemplateLoader
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<string> _foldersToSearch;
        public DefaultTemplateLoader(IEnumerable<string> foldersToSearch, ILogger logger)
        {
            _foldersToSearch = foldersToSearch ?? throw new ArgumentNullException(nameof(foldersToSearch));
            _logger = logger;
        }
        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            foreach(var folder in _foldersToSearch)
            {
                var potentialPath = System.IO.Path.Combine(folder, templateName);
                if (System.IO.File.Exists(potentialPath))
                {
                    _logger.LogDebug("Found template '{TemplateName}' at path: {TemplatePath}", templateName, potentialPath);
                    return potentialPath;
                }
            }
            _logger.LogDebug("Getting path for template: {TemplateName}", templateName);
            return null;
        }

        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            _logger.LogDebug("Loading template from path: {TemplatePath}", templatePath);
            return System.IO.File.ReadAllText(templatePath);
        }

        public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            _logger.LogDebug("Loading template asynchronously from path: {TemplatePath}", templatePath);
            return await System.IO.File.ReadAllTextAsync(templatePath);
        }
    }
}
