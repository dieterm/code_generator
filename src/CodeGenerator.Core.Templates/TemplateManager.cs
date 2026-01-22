using CodeGenerator.Core.Artifacts.FileSystem;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    public class TemplateManager
    {
        private readonly List<string> _registeredTemplateFolders = new List<string>();
        private readonly ILogger<TemplateManager> _logger;
        private readonly TemplateEngineManager _templateEngineManager;
        private readonly Dictionary<string, ITemplate> _templates = new Dictionary<string, ITemplate>();
        public IEnumerable<string> TemplateFolders { get { return _registeredTemplateFolders; } }
        public TemplateManager(ILogger<TemplateManager> logger, TemplateEngineManager templateEngineManager)
        {
            _logger = logger;
            _templateEngineManager = templateEngineManager;
        }

        public void RegisterTemplateFolder(string folderPath)
        {
            if(!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"The directory '{folderPath}' does not exist.");

            if (!_registeredTemplateFolders.Contains(folderPath)) { 
                _registeredTemplateFolders.Add(folderPath);
                // Scan the folder for templates
                ScanFolderForTemplates(folderPath, _templates);
            }
        }

        public IEnumerable<ITemplate> GetAllTemplates()
        {
            return _templates.Values;
        }

        public IEnumerable<ITemplate> GetTemplatesByType(TemplateType templateType)
        {
            return _templates.Values.Where(t => t.TemplateType == templateType).ToList();
        }

        public IEnumerable<ITemplate> GetTemplatesByType(IEnumerable<TemplateType> templateTypes)
        {
            return _templates.Values.Where(t => templateTypes.Contains(t.TemplateType)).ToList();
        }

        public void UnregisterTemplateFolder(string folderPath)
        {
            _registeredTemplateFolders.Remove(folderPath);
            // Optionally, remove templates loaded from this folder
            foreach(var templatePath in _templates.Keys.ToArray())
            {
                if (Path.GetDirectoryName(templatePath)?.Equals(folderPath, StringComparison.OrdinalIgnoreCase) == true)
                {
                    var template = _templates[templatePath];
                    _templates.Remove(templatePath);
                    _logger.LogInformation("Unloaded template: {TemplateId} from {FilePath}", template.TemplateId, templatePath);
                }
            }
        }

        public void RefreshTemplates()
        {
            _templates.Clear();
            foreach(var folder in _registeredTemplateFolders)
            {
                ScanFolderForTemplates(folder, _templates);
            }
        }

        public ITemplate? GetTemplateById(string templateId)
        {
            return _templates.Values.FirstOrDefault(t => t.TemplateId.Equals(templateId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Recursively scan a folder for template files
        /// </summary>
        private void ScanFolderForTemplates(string folderPath, Dictionary<string, ITemplate> templates)
        {
            try
            {
                // Process subdirectories first
                foreach (var subDir in Directory.GetDirectories(folderPath))
                {
                    // Recursively scan subdirectory
                    ScanFolderForTemplates(subDir, templates);
                }

                // Process template files
                foreach (var filePath in Directory.GetFiles(folderPath))
                {
                    // Skip definition files
                    if (filePath.EndsWith(TemplateDefinition.DefinitionFileExtension, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if(_templates.ContainsKey(filePath))
                        continue; // already loaded

                    var extension = Path.GetExtension(filePath).TrimStart('.');
                    var templateEngine = _templateEngineManager.GetTemplateEngineByFileExtension(extension);

                    if (templateEngine != null)
                    {
                        var templateArtifact = templateEngine.CreateTemplateFromFile(filePath);
                        if(templateArtifact!=null)
                        {
                            templates.Add(filePath,templateArtifact);
                            _logger.LogInformation("Loaded template: {TemplateId} from {FilePath}", templateArtifact.TemplateId, filePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning folder for templates: {FolderPath}", folderPath);
            }
        }
    }
}
