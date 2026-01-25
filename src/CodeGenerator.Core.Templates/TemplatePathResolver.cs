using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Templates;

/// <summary>
/// Resolves TemplateId strings to actual file system paths.
/// Supports workspace-specific template overrides and fallback to default templates.
/// </summary>
public class TemplatePathResolver
{
    /// <summary>
    /// Name of the Templates subfolder in workspace directories
    /// </summary>
    public const string TemplatesFolderName = "Templates";

    private readonly ILogger<TemplatePathResolver>? _logger;
    private readonly Dictionary<string, SpecialFolderRegistration> _registeredSpecialFolders = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _registeredRequiredTemplates = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Current workspace directory (set when workspace is opened)
    /// </summary>
    public string? CurrentWorkspaceDirectory { get; private set; }

    /// <summary>
    /// Default template folder from settings
    /// </summary>
    public string? DefaultTemplateFolder { get; private set; }

    /// <summary>
    /// Event raised when a required template folder is missing
    /// </summary>
    public event EventHandler<string>? RequiredTemplateFolderMissing;

    public TemplatePathResolver(ILogger<TemplatePathResolver>? logger = null)
    {
        _logger = logger;
        RegisterDefaultSpecialFolders();
    }

    /// <summary>
    /// Register the default special folders
    /// </summary>
    private void RegisterDefaultSpecialFolders()
    {
        RegisterSpecialFolder(TemplateIdParser.SpecialFolders.Workspace);
        RegisterSpecialFolder(TemplateIdParser.SpecialFolders.Generators);
        RegisterSpecialFolder(TemplateIdParser.SpecialFolders.TemplateEngines);
        RegisterSpecialFolder(TemplateIdParser.SpecialFolders.UserDefined);
    }

    #region Special Folder Registration

    /// <summary>
    /// Registration info for a special folder
    /// </summary>
    public class SpecialFolderRegistration
    {
        public string FolderName { get; init; } = string.Empty;
        public string? ParentSpecialFolder { get; init; }
        public bool IsRootFolder => ParentSpecialFolder == null;
    }

    /// <summary>
    /// Register a special folder that should be created in template directories
    /// </summary>
    /// <param name="folderName">Folder name without @ prefix</param>
    /// <param name="parentSpecialFolder">Parent special folder name without @ prefix, or null for root</param>
    public void RegisterSpecialFolder(string folderName, string? parentSpecialFolder = null)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            throw new ArgumentException("Folder name cannot be empty", nameof(folderName));

        // Remove @ prefix if present
        folderName = folderName.TrimStart('@');
        parentSpecialFolder = parentSpecialFolder?.TrimStart('@');

        if (_registeredSpecialFolders.ContainsKey(folderName))
        {
            _logger?.LogDebug("Special folder '{FolderName}' is already registered", folderName);
            return;
        }

        _registeredSpecialFolders[folderName] = new SpecialFolderRegistration
        {
            FolderName = folderName,
            ParentSpecialFolder = parentSpecialFolder
        };
        
        _logger?.LogDebug("Registered special folder: {FolderName} (parent: {Parent})", folderName, parentSpecialFolder ?? "root");

        // Automatically create the folder in existing template locations
        EnsureSpecialFolderExists(folderName, parentSpecialFolder);
    }

    /// <summary>
    /// Ensure a specific special folder exists in all configured template locations
    /// </summary>
    private void EnsureSpecialFolderExists(string folderName, string? parentSpecialFolder)
    {
        // Create in workspace if set
        if (!string.IsNullOrEmpty(CurrentWorkspaceDirectory))
        {
            var templatesRoot = Path.Combine(CurrentWorkspaceDirectory, TemplatesFolderName);
            CreateSpecialFolderInRoot(templatesRoot, folderName, parentSpecialFolder);
        }

        // Create in default template folder if set
        if (!string.IsNullOrEmpty(DefaultTemplateFolder))
        {
            CreateSpecialFolderInRoot(DefaultTemplateFolder, folderName, parentSpecialFolder);
        }
    }

    /// <summary>
    /// Create a special folder within a templates root directory
    /// </summary>
    private void CreateSpecialFolderInRoot(string templatesRoot, string folderName, string? parentSpecialFolder)
    {
        try
        {
            if (!Directory.Exists(templatesRoot))
                return; // Don't create if templates root doesn't exist

            string folderPath;
            if (string.IsNullOrEmpty(parentSpecialFolder))
            {
                // Root-level special folder
                folderPath = Path.Combine(templatesRoot, folderName);
            }
            else
            {
                // Nested special folder - find parent path
                var parentPath = GetSpecialFolderPath(templatesRoot, parentSpecialFolder);
                if (string.IsNullOrEmpty(parentPath) || !Directory.Exists(parentPath))
                    return;
                    
                folderPath = Path.Combine(parentPath, folderName);
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                _logger?.LogDebug("Created special folder: {Path}", folderPath);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create special folder '{FolderName}' in {Root}", folderName, templatesRoot);
        }
    }

    /// <summary>
    /// Register a required template that should be available.
    /// This will auto-register special folders and create all necessary subfolders.
    /// </summary>
    /// <param name="templateId">Full TemplateId</param>
    public void RegisterRequiredTemplate(string templateId)
    {
        if (string.IsNullOrWhiteSpace(templateId))
            return;

        _registeredRequiredTemplates.Add(templateId);

        // Parse the template ID to get all path segments
        var parsed = TemplateIdParser.Parse(templateId);
        if (!parsed.IsValid)
        {
            _logger?.LogWarning("Invalid TemplateId for registration: {TemplateId}", templateId);
            return;
        }

        // Auto-register special folders from the TemplateId
        if (parsed.HasSpecialFolders)
        {
            string? previousFolder = null;
            foreach (var specialFolder in parsed.SpecialFolderSegments)
            {
                if (!_registeredSpecialFolders.ContainsKey(specialFolder))
                {
                    RegisterSpecialFolder(specialFolder, previousFolder);
                }
                previousFolder = specialFolder;
            }
        }

        // Create the full folder path for this template (including non-special subfolders)
        EnsureTemplatePathExists(parsed);

        _logger?.LogDebug("Registered required template: {TemplateId}", templateId);
    }

    /// <summary>
    /// Ensure the full folder path for a template exists (including all subfolders)
    /// </summary>
    private void EnsureTemplatePathExists(ParsedTemplateId parsed)
    {
        // Create in workspace if set
        if (!string.IsNullOrEmpty(CurrentWorkspaceDirectory))
        {
            var templatesRoot = Path.Combine(CurrentWorkspaceDirectory, TemplatesFolderName);
            CreateTemplateFolderPath(templatesRoot, parsed);
        }

        // Create in default template folder if set
        if (!string.IsNullOrEmpty(DefaultTemplateFolder))
        {
            CreateTemplateFolderPath(DefaultTemplateFolder, parsed);
        }
    }

    /// <summary>
    /// Create the full folder path for a template, including all intermediate folders
    /// </summary>
    private void CreateTemplateFolderPath(string templatesRoot, ParsedTemplateId parsed)
    {
        try
        {
            if (!Directory.Exists(templatesRoot))
                return;

            var currentPath = templatesRoot;

            // Create each path segment
            foreach (var segment in parsed.PathSegments)
            {
                var folderName = segment.TrimStart('@');
                currentPath = Path.Combine(currentPath, folderName);

                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                    _logger?.LogDebug("Created template subfolder: {Path}", currentPath);
                }
            }

            // Also create the template name folder (where template files will be stored)
            var templateFolder = Path.Combine(currentPath, parsed.TemplateName);
            if (!Directory.Exists(templateFolder))
            {
                Directory.CreateDirectory(templateFolder);
                _logger?.LogDebug("Created template folder: {Path}", templateFolder);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create template path for '{TemplateId}' in {Root}", 
                parsed.FullTemplateId, templatesRoot);
        }
    }

    /// <summary>
    /// Get all registered special folders
    /// </summary>
    public IEnumerable<SpecialFolderRegistration> GetRegisteredSpecialFolders()
    {
        return _registeredSpecialFolders.Values;
    }

    /// <summary>
    /// Get root-level special folders
    /// </summary>
    public IEnumerable<SpecialFolderRegistration> GetRootSpecialFolders()
    {
        return _registeredSpecialFolders.Values.Where(f => f.IsRootFolder);
    }

    #endregion

    #region Workspace and Default Folder Management

    /// <summary>
    /// Set the current workspace directory and ensure template folders exist
    /// </summary>
    public void SetWorkspaceDirectory(string? workspaceDirectory)
    {
        CurrentWorkspaceDirectory = workspaceDirectory;

        if (!string.IsNullOrEmpty(workspaceDirectory))
        {
            EnsureTemplateFoldersExist(workspaceDirectory);
            // Also ensure all registered required template paths exist
            EnsureAllRegisteredTemplatePathsExist(workspaceDirectory, useTemplatesSubfolder: true);
            _logger?.LogInformation("Set workspace directory: {Directory}", workspaceDirectory);
        }
    }

    /// <summary>
    /// Set the default template folder from settings and ensure folders exist
    /// </summary>
    public void SetDefaultTemplateFolder(string? defaultFolder)
    {
        DefaultTemplateFolder = defaultFolder;

        if (!string.IsNullOrEmpty(defaultFolder))
        {
            EnsureTemplateFoldersExist(defaultFolder, isDefaultFolder: true);
            // Also ensure all registered required template paths exist
            EnsureAllRegisteredTemplatePathsExist(defaultFolder, useTemplatesSubfolder: false);
            _logger?.LogInformation("Set default template folder: {Folder}", defaultFolder);
        }
    }

    /// <summary>
    /// Ensure all registered required template paths exist in a directory
    /// </summary>
    private void EnsureAllRegisteredTemplatePathsExist(string rootDirectory, bool useTemplatesSubfolder)
    {
        var templatesRoot = useTemplatesSubfolder 
            ? Path.Combine(rootDirectory, TemplatesFolderName) 
            : rootDirectory;

        if (!Directory.Exists(templatesRoot))
            return;

        foreach (var templateId in _registeredRequiredTemplates)
        {
            var parsed = TemplateIdParser.Parse(templateId);
            if (parsed.IsValid)
            {
                CreateTemplateFolderPath(templatesRoot, parsed);
            }
        }
    }

    /// <summary>
    /// Ensure template folders and special folders exist in the given root directory
    /// </summary>
    /// <param name="rootDirectory">Root directory (workspace or default template folder)</param>
    /// <param name="isDefaultFolder">If true, folders are created directly; otherwise, in Templates subfolder</param>
    public void EnsureTemplateFoldersExist(string rootDirectory, bool isDefaultFolder = false)
    {
        if (string.IsNullOrEmpty(rootDirectory))
            return;

        try
        {
            var templatesRoot = isDefaultFolder ? rootDirectory : Path.Combine(rootDirectory, TemplatesFolderName);

            // Create Templates folder if needed
            if (!Directory.Exists(templatesRoot))
            {
                Directory.CreateDirectory(templatesRoot);
                _logger?.LogInformation("Created templates folder: {Path}", templatesRoot);
            }

            // Create root special folders first
            foreach (var folder in GetRootSpecialFolders())
            {
                var folderPath = Path.Combine(templatesRoot, folder.FolderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    _logger?.LogDebug("Created special folder: {Path}", folderPath);
                }
            }

            // Create nested special folders (need to process in order of dependency)
            var processedFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var folder in GetRootSpecialFolders())
            {
                processedFolders.Add(folder.FolderName);
            }

            // Keep processing until all folders are created
            bool madeProgress = true;
            while (madeProgress)
            {
                madeProgress = false;
                foreach (var folder in _registeredSpecialFolders.Values.Where(f => !f.IsRootFolder && !processedFolders.Contains(f.FolderName)))
                {
                    // Check if parent is already processed
                    if (processedFolders.Contains(folder.ParentSpecialFolder!))
                    {
                        var parentPath = GetSpecialFolderPath(templatesRoot, folder.ParentSpecialFolder!);
                        if (!string.IsNullOrEmpty(parentPath))
                        {
                            var folderPath = Path.Combine(parentPath, folder.FolderName);
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                                _logger?.LogDebug("Created nested special folder: {Path}", folderPath);
                            }
                        }
                        processedFolders.Add(folder.FolderName);
                        madeProgress = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create template folders in {Directory}", rootDirectory);
        }
    }

    private string? GetSpecialFolderPath(string templatesRoot, string folderName)
    {
        if (!_registeredSpecialFolders.TryGetValue(folderName, out var registration))
            return null;

        if (registration.IsRootFolder)
        {
            return Path.Combine(templatesRoot, registration.FolderName);
        }
        else
        {
            var parentPath = GetSpecialFolderPath(templatesRoot, registration.ParentSpecialFolder!);
            return parentPath != null ? Path.Combine(parentPath, registration.FolderName) : null;
        }
    }

    #endregion

    #region Path Resolution

    /// <summary>
    /// Resolve a TemplateId to a file path, checking workspace first, then default folder
    /// </summary>
    /// <param name="templateId">The TemplateId to resolve</param>
    /// <returns>Resolved file path, or null if not found</returns>
    public string? ResolveTemplateId(string templateId)
    {
        if (string.IsNullOrWhiteSpace(templateId))
            return null;

        // If it doesn't use special folder syntax, return as-is (might be a direct path)
        if (!TemplateIdParser.HasSpecialFolderSyntax(templateId))
        {
            return File.Exists(templateId) ? templateId : null;
        }

        var parsed = TemplateIdParser.Parse(templateId);
        if (!parsed.IsValid)
        {
            _logger?.LogWarning("Invalid TemplateId: {TemplateId} - {Error}", templateId, parsed.ErrorMessage);
            return null;
        }

        // Try workspace templates first
        if (!string.IsNullOrEmpty(CurrentWorkspaceDirectory))
        {
            var workspacePath = ResolveInDirectory(parsed, CurrentWorkspaceDirectory, useTemplatesSubfolder: true);
            if (!string.IsNullOrEmpty(workspacePath))
            {
                _logger?.LogDebug("Resolved template '{TemplateId}' in workspace: {Path}", templateId, workspacePath);
                return workspacePath;
            }
        }

        // Fallback to default template folder
        if (!string.IsNullOrEmpty(DefaultTemplateFolder))
        {
            var defaultPath = ResolveInDirectory(parsed, DefaultTemplateFolder, useTemplatesSubfolder: false);
            if (!string.IsNullOrEmpty(defaultPath))
            {
                _logger?.LogDebug("Resolved template '{TemplateId}' in default folder: {Path}", templateId, defaultPath);
                return defaultPath;
            }
        }

        _logger?.LogWarning("Could not resolve template: {TemplateId}", templateId);
        return null;
    }

    /// <summary>
    /// Resolve a TemplateId to a folder path (for template folders)
    /// </summary>
    public string? ResolveTemplateFolderId(string templateId)
    {
        if (string.IsNullOrWhiteSpace(templateId))
            return null;

        if (!TemplateIdParser.HasSpecialFolderSyntax(templateId))
        {
            return Directory.Exists(templateId) ? templateId : null;
        }

        var parsed = TemplateIdParser.Parse(templateId);
        if (!parsed.IsValid)
            return null;

        // Try workspace first
        if (!string.IsNullOrEmpty(CurrentWorkspaceDirectory))
        {
            var workspacePath = ResolveFolderInDirectory(parsed, CurrentWorkspaceDirectory, useTemplatesSubfolder: true);
            if (!string.IsNullOrEmpty(workspacePath))
                return workspacePath;
        }

        // Fallback to default
        if (!string.IsNullOrEmpty(DefaultTemplateFolder))
        {
            var defaultPath = ResolveFolderInDirectory(parsed, DefaultTemplateFolder, useTemplatesSubfolder: false);
            if (!string.IsNullOrEmpty(defaultPath))
                return defaultPath;
        }

        return null;
    }

    /// <summary>
    /// Get the expected file path for a TemplateId (even if file doesn't exist yet)
    /// </summary>
    public string GetExpectedTemplatePath(string templateId, bool preferWorkspace = true)
    {
        var parsed = TemplateIdParser.Parse(templateId);
        if (!parsed.IsValid)
            throw new ArgumentException($"Invalid TemplateId: {parsed.ErrorMessage}", nameof(templateId));

        string rootDir;
        bool useSubfolder;

        if (preferWorkspace && !string.IsNullOrEmpty(CurrentWorkspaceDirectory))
        {
            rootDir = CurrentWorkspaceDirectory;
            useSubfolder = true;
        }
        else if (!string.IsNullOrEmpty(DefaultTemplateFolder))
        {
            rootDir = DefaultTemplateFolder;
            useSubfolder = false;
        }
        else
        {
            throw new InvalidOperationException("No workspace or default template folder is configured");
        }

        return BuildPathFromParsed(parsed, rootDir, useSubfolder);
    }

    private string? ResolveInDirectory(ParsedTemplateId parsed, string rootDirectory, bool useTemplatesSubfolder)
    {
        var folderPath = BuildFolderPathFromParsed(parsed, rootDirectory, useTemplatesSubfolder);
        
        if (!Directory.Exists(folderPath))
            return null;

        // Look for template files in the template folder
        var templateFolder = Path.Combine(folderPath, parsed.TemplateName);
        if (Directory.Exists(templateFolder))
        {
            // Find template files in the folder
            var defFiles = Directory.GetFiles(templateFolder)
                .Where(f => f.EndsWith(TemplateDefinition.DefinitionFileExtension, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach(var defFile in defFiles)
            {
                var templateDef = TemplateDefinition.LoadFromFile(defFile);
                if (templateDef != null && string.Equals(templateDef.TemplateName, parsed.TemplateName, StringComparison.OrdinalIgnoreCase))
                {
                    // remove definition file extension to get template file path
                    return defFile.Substring(0, defFile.Length - TemplateDefinition.DefinitionFileExtension.Length);

                }
            }
        }

        return null;
    }

    private string? ResolveFolderInDirectory(ParsedTemplateId parsed, string rootDirectory, bool useTemplatesSubfolder)
    {
        var folderPath = BuildFolderPathFromParsed(parsed, rootDirectory, useTemplatesSubfolder);
        var templateFolder = Path.Combine(folderPath, parsed.TemplateName);
        
        return Directory.Exists(templateFolder) ? templateFolder : null;
    }

    private string BuildPathFromParsed(ParsedTemplateId parsed, string rootDirectory, bool useTemplatesSubfolder)
    {
        var folderPath = BuildFolderPathFromParsed(parsed, rootDirectory, useTemplatesSubfolder);
        return Path.Combine(folderPath, parsed.TemplateName);
    }

    private string BuildFolderPathFromParsed(ParsedTemplateId parsed, string rootDirectory, bool useTemplatesSubfolder)
    {
        var parts = new List<string> { rootDirectory };

        if (useTemplatesSubfolder)
        {
            parts.Add(TemplatesFolderName);
        }

        // Convert path segments to folder path (remove @ prefix from special folders)
        foreach (var segment in parsed.PathSegments)
        {
            parts.Add(segment.TrimStart('@'));
        }

        return Path.Combine(parts.ToArray());
    }

    #endregion

    #region Template Location Helpers

    /// <summary>
    /// Get all possible locations for a TemplateId (workspace first, then default)
    /// </summary>
    public IEnumerable<string> GetPossibleLocations(string templateId)
    {
        var parsed = TemplateIdParser.Parse(templateId);
        if (!parsed.IsValid)
            yield break;

        if (!string.IsNullOrEmpty(CurrentWorkspaceDirectory))
        {
            yield return BuildPathFromParsed(parsed, CurrentWorkspaceDirectory, useTemplatesSubfolder: true);
        }

        if (!string.IsNullOrEmpty(DefaultTemplateFolder))
        {
            yield return BuildPathFromParsed(parsed, DefaultTemplateFolder, useTemplatesSubfolder: false);
        }
    }

    /// <summary>
    /// Check if a template exists at any location
    /// </summary>
    public bool TemplateExists(string templateId)
    {
        return ResolveTemplateId(templateId) != null;
    }

    #endregion
}
