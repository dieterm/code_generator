using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
//using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Settings;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;

namespace CodeGenerator.Core.Workspaces.Services
{
    /// <summary>
    /// Service for loading and saving workspace files
    /// </summary>
    public class WorkspaceFileService
    {
        public const string WorkspaceFileExtension = ".cg";
        public const string CodeGeneratorFileDialogFilter = $"CodeGenerator Workspace Files (*{WorkspaceFileExtension})|*{WorkspaceFileExtension}|All Files (*.*)|*.*";

        private readonly ILogger<WorkspaceFileService> _logger;
        private readonly IDatasourceFactory _datasourceFactory;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new DictionaryStringObjectJsonConverter() }
        };
        
        
        public WorkspaceFileService(ILogger<WorkspaceFileService> logger, IDatasourceFactory datasourceFactory)
        {
            _logger = logger;
            _datasourceFactory = datasourceFactory;
        }
        public async Task<string> LoadWorkspaceName(string filePath, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Workspace file not found: {filePath}");
            }
            _logger.LogInformation("Loading workspace summary from {FilePath}", filePath);

            var json = await File.ReadAllTextAsync(filePath, cancellationToken);

            var workspaceState = JsonSerializer.Deserialize<ArtifactNoChildrenState>(json, JsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize workspace state");

            return workspaceState.Properties[nameof(WorkspaceArtifact.Name)] as string;
        }

        /// <summary>
        /// Load a workspace from a .codegenerator file
        /// </summary>
        public async Task<WorkspaceArtifact> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Workspace file not found: {filePath}");
            }

            _logger.LogInformation("Loading workspace from {FilePath}", filePath);

            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            
            var workspaceState = JsonSerializer.Deserialize<ArtifactState>(json, JsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize workspace state");
            var workspaceArtifact = (WorkspaceArtifact)ArtifactFactory.CreateArtifact(workspaceState);
            
            _logger.LogInformation("Loaded workspace '{Name}' with {Count} datasources", 
                workspaceArtifact.Name, workspaceArtifact.Datasources.GetDatasources().Count());
            
            return workspaceArtifact;
        }

        /// <summary>
        /// Save a workspace to a .cg file
        /// </summary>
        public async Task SaveAsync(WorkspaceArtifact workspace, string? filePath = null, CancellationToken cancellationToken = default)
        {
            var targetPath = filePath ?? workspace.WorkspaceFilePath;
            
            if (string.IsNullOrEmpty(targetPath))
            {
                throw new InvalidOperationException("Workspace file path is not set");
            }

            _logger.LogInformation("Saving workspace to {FilePath}", targetPath);

            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var workspaceState = (ArtifactState)workspace.CaptureState();
            var json = JsonSerializer.Serialize(workspaceState, JsonOptions);
            await File.WriteAllTextAsync(targetPath, json, cancellationToken);

            workspace.WorkspaceFilePath = targetPath;

            _logger.LogInformation("Saved workspace '{Name}'", workspace.Name);
        }

        public string GetFilePath(string directory, string name)
        {
            return Path.Combine(directory, $"{name}{WorkspaceFileExtension}");
        }

        ///// <summary>
        ///// Create a new workspace in a directory
        ///// </summary>
        //public async Task<WorkspaceArtifact> CreateNewAsync(string directory, string name = "Workspace", CancellationToken cancellationToken = default)
        //{
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }

        //    var filePath = Path.Combine(directory,$"{name}{WorkspaceFileExtension}");
        //    // get default settings for new workspace
        //    var workspaceSettings = WorkspaceSettings.Instance;
        //    var workspace = new WorkspaceArtifact(name)
        //    {
        //        WorkspaceFilePath = filePath,
        //        RootNamespace = workspaceSettings.RootNamespace,
        //        OutputDirectory = workspaceSettings.DefaultOutputDirectory,
        //        DefaultTargetFramework = workspaceSettings.DefaultTargetFramework,
        //        DefaultLanguage = workspaceSettings.DefaultLanguage,
        //        CodeArchitectureId = workspaceSettings.DefaultCodeArchitectureId,
        //        DependencyInjectionFrameworkId = workspaceSettings.DefaultDependencyInjectionFrameworkId
        //    };

        //    await SaveAsync(workspace, filePath, cancellationToken);

        //    _logger.LogInformation("Created new workspace '{Name}' at {Directory}", name, directory);

        //    return workspace;
        //}

        /// <summary>
        /// Check if a directory contains a workspace
        /// </summary>
        public bool IsWorkspaceDirectory(string directory)
        {
            var workspaceFile = GetWorkspaceFilePath(directory);
            return File.Exists(workspaceFile);
        }

        /// <summary>
        /// Get the workspace file path for a directory
        /// </summary>
        public string GetWorkspaceFilePath(string directory)
        {
            return Path.Combine(directory, WorkspaceFileExtension);
        }

        /// <summary>
        /// Returns a default filename for new workspaces.
        /// Eg. "NewWorkspace.cg"
        /// </summary>
        /// <returns></returns>
        public static string? GetDefaultFilename()
        {
            return $"NewWorkspace{WorkspaceFileExtension}";
        }
    }
}
