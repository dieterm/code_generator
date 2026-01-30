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
        private readonly TemplatePathResolver _pathResolver;
        private readonly Dictionary<string, ITemplate> _templates = new Dictionary<string, ITemplate>();
        private readonly object _templatesLock = new object();
        private readonly List<Task> _pendingScanTasks = new List<Task>();
        
        public IEnumerable<string> TemplateFolders { get { return _registeredTemplateFolders; } }
        
        /// <summary>
        /// Gets the template path resolver for special folder syntax support
        /// </summary>
        public TemplatePathResolver PathResolver => _pathResolver;

        /// <summary>
        /// Event raised when template scanning is complete
        /// </summary>
        public event EventHandler? TemplatesLoaded;

        public TemplateManager(ILogger<TemplateManager> logger, TemplateEngineManager templateEngineManager)
        {
            _logger = logger;
            _templateEngineManager = templateEngineManager;
            _pathResolver = new TemplatePathResolver(
                logger != null ? Microsoft.Extensions.Logging.LoggerFactory.Create(b => { }).CreateLogger<TemplatePathResolver>() : null);
        }

        /// <summary>
        /// Register a special folder that should be created in template directories
        /// </summary>
        /// <param name="folderName">Folder name without @ prefix</param>
        /// <param name="parentSpecialFolder">Parent special folder name without @ prefix, or null for root</param>
        public void RegisterSpecialFolder(string folderName, string? parentSpecialFolder = null)
        {
            _pathResolver.RegisterSpecialFolder(folderName, parentSpecialFolder);
        }

        /// <summary>
        /// Register a required template that should be available
        /// </summary>
        /// <param name="templateId">Full TemplateId using special folder syntax</param>
        public void RegisterRequiredTemplate(string templateId)
        {
            _pathResolver.RegisterRequiredTemplate(templateId);
        }

        /// <summary>
        /// Register multiple required templates
        /// </summary>
        public void RegisterRequiredTemplates(IEnumerable<TemplateDefinitionAndLocation> templates)
        {
            foreach (var template in templates)
            {
                RegisterRequiredTemplate(template.TemplateId);
            }
        }

        /// <summary>
        /// Set the default template folder from settings
        /// </summary>
        public void SetDefaultTemplateFolder(string? defaultFolder)
        {
            _pathResolver.SetDefaultTemplateFolder(defaultFolder);
            
            if (!string.IsNullOrEmpty(defaultFolder) && Directory.Exists(defaultFolder))
            {
                // Scan default folder for templates on background thread
                if (!_registeredTemplateFolders.Contains(defaultFolder))
                {
                    _registeredTemplateFolders.Add(defaultFolder);
                    var scanTask = Task.Run(() => ScanFolderForTemplates(defaultFolder));
                    lock (_pendingScanTasks)
                    {
                        _pendingScanTasks.Add(scanTask);
                    }
                    // Fire and forget, but track completion
                    scanTask.ContinueWith(t => OnScanTaskCompleted(t), TaskScheduler.Default);
                }
            }
        }

        /// <summary>
        /// Set the current workspace directory
        /// </summary>
        public void SetWorkspaceDirectory(string? workspaceDirectory)
        {
            // Clear previous workspace templates if any
            var previousWorkspace = _pathResolver.CurrentWorkspaceDirectory;
            if (!string.IsNullOrEmpty(previousWorkspace))
            {
                var workspaceTemplatesFolder = Path.Combine(previousWorkspace, TemplatePathResolver.TemplatesFolderName);
                if (_registeredTemplateFolders.Contains(workspaceTemplatesFolder))
                {
                    UnregisterTemplateFolder(workspaceTemplatesFolder);
                }
            }

            _pathResolver.SetWorkspaceDirectory(workspaceDirectory);

            if (!string.IsNullOrEmpty(workspaceDirectory))
            {
                var templatesFolder = Path.Combine(workspaceDirectory, TemplatePathResolver.TemplatesFolderName);
                if (Directory.Exists(templatesFolder) && !_registeredTemplateFolders.Contains(templatesFolder))
                {
                    _registeredTemplateFolders.Insert(0, templatesFolder); // Workspace takes priority
                    var scanTask = Task.Run(() => ScanFolderForTemplates(templatesFolder));
                    lock (_pendingScanTasks)
                    {
                        _pendingScanTasks.Add(scanTask);
                    }
                    scanTask.ContinueWith(t => OnScanTaskCompleted(t), TaskScheduler.Default);
                }
            }
        }

        public void RegisterTemplateFolder(string folderPath)
        {
            if(!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"The directory '{folderPath}' does not exist.");

            if (!_registeredTemplateFolders.Contains(folderPath)) { 
                _registeredTemplateFolders.Add(folderPath);
                // Scan the folder for templates on background thread
                var scanTask = Task.Run(() => ScanFolderForTemplates(folderPath));
                lock (_pendingScanTasks)
                {
                    _pendingScanTasks.Add(scanTask);
                }
                scanTask.ContinueWith(t => OnScanTaskCompleted(t), TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Wait for all pending template scans to complete
        /// </summary>
        public async Task WaitForPendingScansAsync()
        {
            Task[] tasksToWait;
            lock (_pendingScanTasks)
            {
                tasksToWait = _pendingScanTasks.ToArray();
            }
            if (tasksToWait.Length > 0)
            {
                await Task.WhenAll(tasksToWait);
            }
        }

        private void OnScanTaskCompleted(Task task)
        {
            lock (_pendingScanTasks)
            {
                _pendingScanTasks.Remove(task);
                if (_pendingScanTasks.Count == 0)
                {
                    // All scans complete, notify listeners
                    TemplatesLoaded?.Invoke(this, EventArgs.Empty);
                }
            }

            if (task.IsFaulted && task.Exception != null)
            {
                _logger.LogError(task.Exception, "Error during template folder scan");
            }
        }

        public IEnumerable<ITemplate> GetAllTemplates()
        {
            lock (_templatesLock)
            {
                return _templates.Values.ToList();
            }
        }

        public IEnumerable<ITemplate> GetTemplatesByType(TemplateType templateType)
        {
            lock (_templatesLock)
            {
                return _templates.Values.Where(t => t.TemplateType == templateType).ToList();
            }
        }

        public IEnumerable<ITemplate> GetTemplatesByType(IEnumerable<TemplateType> templateTypes)
        {
            lock (_templatesLock)
            {
                return _templates.Values.Where(t => templateTypes.Contains(t.TemplateType)).ToList();
            }
        }

        public void UnregisterTemplateFolder(string folderPath)
        {
            _registeredTemplateFolders.Remove(folderPath);
            // Optionally, remove templates loaded from this folder
            lock (_templatesLock)
            {
                foreach (var templatePath in _templates.Keys.ToArray())
                {
                    if (Path.GetDirectoryName(templatePath)?.Equals(folderPath, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        var template = _templates[templatePath];
                        _templates.Remove(templatePath);
                        _logger.LogInformation("Unloaded template: {TemplateId} from {FilePath}", template.TemplateId, templatePath);
                    }
                }
            }
        }

        public void RefreshTemplates()
        {
            lock (_templatesLock)
            {
                _templates.Clear();
            }
            foreach (var folder in _registeredTemplateFolders)
            {
                var scanTask = Task.Run(() => ScanFolderForTemplates(folder));
                lock (_pendingScanTasks)
                {
                    _pendingScanTasks.Add(scanTask);
                }
                scanTask.ContinueWith(t => OnScanTaskCompleted(t), TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Get a template by its ID, using special folder syntax resolution.
        /// First searches in workspace templates, then falls back to default templates.
        /// </summary>
        /// <param name="templateId">Template ID, optionally using @folder syntax</param>
        /// <returns>The template if found, null otherwise</returns>
        public ITemplate? GetTemplateById(string templateId)
        {
            if (string.IsNullOrWhiteSpace(templateId))
                return null;

            // If using special folder syntax, resolve the path first
            if (TemplateIdParser.HasSpecialFolderSyntax(templateId))
            {
                var resolvedPath = _pathResolver.ResolveTemplateId(templateId);
                if (!string.IsNullOrEmpty(resolvedPath))
                {
                    // Check if we already have this template loaded
                    lock (_templatesLock)
                    {
                        if (_templates.TryGetValue(resolvedPath, out var cachedTemplate))
                        {
                            return cachedTemplate;
                        }
                    }

                    // Load the template from the resolved path
                    var template = LoadTemplateFromPath(resolvedPath);
                    if (template != null)
                    {
                        lock (_templatesLock)
                        {
                            _templates[resolvedPath] = template;
                        }
                        return template;
                    }
                }
            }

            // Standard lookup by template ID
            lock (_templatesLock)
            {
                var result = _templates.Values.FirstOrDefault(t => t.TemplateId.Equals(templateId, StringComparison.OrdinalIgnoreCase));
                if (result != null)
                    return result;

                // Try to match by the template name portion only (for backward compatibility)
                var parsed = TemplateIdParser.Parse(templateId);
                if (parsed.IsValid && !string.IsNullOrEmpty(parsed.TemplateName))
                {
                    result = _templates.Values.FirstOrDefault(t =>
                        t.TemplateId.Equals(parsed.TemplateName, StringComparison.OrdinalIgnoreCase) ||
                        t.TemplateId.EndsWith("/" + parsed.TemplateName, StringComparison.OrdinalIgnoreCase));
                }

                return result;
            }
        }

        /// <summary>
        /// Resolve a TemplateId to a file path
        /// </summary>
        public string? ResolveTemplateIdToPath(string templateId)
        {
            return _pathResolver.ResolveTemplateId(templateId);
        }

        /// <summary>
        /// Resolve a TemplateId to a folder path
        /// </summary>
        public string? ResolveTemplateIdToFolderPath(string templateId)
        {
            return _pathResolver.ResolveTemplateFolderId(templateId);
        }

        /// <summary>
        /// Check if a template exists
        /// </summary>
        public bool TemplateExists(string templateId)
        {
            return GetTemplateById(templateId) != null || _pathResolver.TemplateExists(templateId);
        }

        /// <summary>
        /// Load a template from a file path
        /// </summary>
        private ITemplate? LoadTemplateFromPath(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                var extension = Path.GetExtension(filePath).TrimStart('.');
                var templateEngine = _templateEngineManager.GetTemplateEngineByFileExtension(extension);

                if (templateEngine != null)
                {
                    var template = templateEngine.CreateTemplateFromFile(filePath);
                    if (template != null)
                    {
                        _logger.LogInformation("Loaded template: {TemplateId} from {FilePath}", template.TemplateId, filePath);
                        return template;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading template from {FilePath}", filePath);
            }

            return null;
        }

        /// <summary>
        /// Recursively scan a folder for template files (thread-safe)
        /// </summary>
        private void ScanFolderForTemplates(string folderPath)
        {
            try
            {
                // Process subdirectories first
                foreach (var subDir in Directory.GetDirectories(folderPath))
                {
                    // Recursively scan subdirectory
                    ScanFolderForTemplates(subDir);
                }

                // Process template files
                foreach (var filePath in Directory.GetFiles(folderPath))
                {
                    // Skip definition files
                    if (filePath.EndsWith(TemplateDefinition.DefinitionFileExtension, StringComparison.OrdinalIgnoreCase))
                        continue;

                    lock (_templatesLock)
                    {
                        if (_templates.ContainsKey(filePath))
                            continue; // already loaded
                    }

                    var extension = Path.GetExtension(filePath).TrimStart('.');
                    var templateEngine = _templateEngineManager.GetTemplateEngineByFileExtension(extension);

                    if (templateEngine != null)
                    {
                        var templateArtifact = templateEngine.CreateTemplateFromFile(filePath);
                        if (templateArtifact != null)
                        {
                            lock (_templatesLock)
                            {
                                if (!_templates.ContainsKey(filePath))
                                {
                                    _templates.Add(filePath, templateArtifact);
                                    _logger.LogInformation("Loaded template: {TemplateId} from {FilePath}", templateArtifact.TemplateId, filePath);
                                }
                            }
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
