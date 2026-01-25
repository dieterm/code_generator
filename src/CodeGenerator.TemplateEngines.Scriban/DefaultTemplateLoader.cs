using CodeGenerator.Core.Templates;
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
        private readonly TemplatePathResolver? _pathResolver;

        public DefaultTemplateLoader(IEnumerable<string> foldersToSearch, ILogger logger, TemplatePathResolver? pathResolver = null)
        {
            _foldersToSearch = foldersToSearch ?? throw new ArgumentNullException(nameof(foldersToSearch));
            _logger = logger;
            _pathResolver = pathResolver;
        }

        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            // Check if the template name uses special folder syntax (@folder/...)
            if (TemplateIdParser.HasSpecialFolderSyntax(templateName))
            {
                return ResolveSpecialFolderTemplate(templateName);
            }

            // Standard folder search
            foreach (var folder in _foldersToSearch)
            {
                var potentialPath = System.IO.Path.Combine(folder, templateName);
                if (System.IO.File.Exists(potentialPath))
                {
                    _logger.LogDebug("Found template '{TemplateName}' at path: {TemplatePath}", templateName, potentialPath);
                    return potentialPath;
                }
            }

            _logger.LogDebug("Template not found in search folders: {TemplateName}", templateName);
            return null;
        }

        /// <summary>
        /// Resolve a template using special folder syntax (@Workspace/@ArtifactName/...)
        /// </summary>
        private string ResolveSpecialFolderTemplate(string templateId)
        {
            if (_pathResolver != null)
            {
                var resolvedPath = _pathResolver.ResolveTemplateId(templateId);
                if (!string.IsNullOrEmpty(resolvedPath))
                {
                    _logger.LogDebug("Resolved special folder template '{TemplateId}' to: {Path}", templateId, resolvedPath);
                    return resolvedPath;
                }

                _logger.LogWarning("Could not resolve special folder template: {TemplateId}", templateId);
            }
            else
            {
                // Fallback: try to resolve manually if no path resolver is available
                var parsed = TemplateIdParser.Parse(templateId);
                if (parsed.IsValid)
                {
                    // Try each search folder
                    foreach (var folder in _foldersToSearch)
                    {
                        var path = BuildPathFromParsed(folder, parsed);
                        if (System.IO.File.Exists(path))
                        {
                            _logger.LogDebug("Found special folder template '{TemplateId}' at: {Path}", templateId, path);
                            return path;
                        }

                        // Also check as a folder with template files inside
                        if (System.IO.Directory.Exists(path))
                        {
                            var files = System.IO.Directory.GetFiles(path)
                                .Where(f => !f.EndsWith(TemplateDefinition.DefinitionFileExtension, StringComparison.OrdinalIgnoreCase))
                                .ToList();

                            if (files.Count > 0)
                            {
                                _logger.LogDebug("Found special folder template '{TemplateId}' in folder: {Path}", templateId, files[0]);
                                return files[0];
                            }
                        }
                    }
                }

                _logger.LogWarning("Could not resolve special folder template (no path resolver): {TemplateId}", templateId);
            }

            return null;
        }

        private string BuildPathFromParsed(string rootFolder, ParsedTemplateId parsed)
        {
            var parts = new List<string> { rootFolder };

            foreach (var segment in parsed.PathSegments)
            {
                parts.Add(segment.TrimStart('@'));
            }

            parts.Add(parsed.TemplateName);

            return System.IO.Path.Combine(parts.ToArray());
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
