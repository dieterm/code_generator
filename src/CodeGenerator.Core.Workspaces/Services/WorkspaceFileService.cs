using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Settings;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CodeGenerator.Core.Workspaces.Services
{
    /// <summary>
    /// Service for loading and saving workspace files
    /// </summary>
    public class WorkspaceFileService
    {
        private readonly ILogger<WorkspaceFileService> _logger;
        private readonly IDatasourceFactory _datasourceFactory;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public const string WorkspaceFileExtension = ".codegenerator";

        public WorkspaceFileService(ILogger<WorkspaceFileService> logger, IDatasourceFactory datasourceFactory)
        {
            _logger = logger;
            _datasourceFactory = datasourceFactory;
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
            var workspaceFile = JsonSerializer.Deserialize<WorkspaceFile>(json, JsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize workspace file");

            var workspace = new WorkspaceArtifact(workspaceFile.Name)
            {
                WorkspaceFilePath = filePath,
                RootNamespace = workspaceFile.RootNamespace,
                DefaultOutputDirectory = workspaceFile.DefaultOutputDirectory,
                DefaultTargetFramework = workspaceFile.DefaultTargetFramework,
                DefaultLanguage = workspaceFile.DefaultLanguage
            };

            // Load datasources
            foreach (var datasourceDef in workspaceFile.Datasources)
            {
                try
                {
                    var datasource = _datasourceFactory.Create(datasourceDef);
                    if (datasource != null)
                    {
                        workspace.Datasources.AddDatasource(datasource);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load datasource {Name} ({Type})", datasourceDef.Name, datasourceDef.Type);
                }
            }

            _logger.LogInformation("Loaded workspace '{Name}' with {Count} datasources", 
                workspace.Name, workspace.Datasources.GetDatasources().Count());

            return workspace;
        }

        /// <summary>
        /// Save a workspace to a .codegenerator file
        /// </summary>
        public async Task SaveAsync(WorkspaceArtifact workspace, string? filePath = null, CancellationToken cancellationToken = default)
        {
            var targetPath = filePath ?? workspace.WorkspaceFilePath;
            
            if (string.IsNullOrEmpty(targetPath))
            {
                throw new InvalidOperationException("Workspace file path is not set");
            }

            _logger.LogInformation("Saving workspace to {FilePath}", targetPath);

            var workspaceFile = new WorkspaceFile
            {
                Name = workspace.Name,
                RootNamespace = workspace.RootNamespace,
                DefaultOutputDirectory = workspace.DefaultOutputDirectory,
                DefaultTargetFramework = workspace.DefaultTargetFramework,
                DefaultLanguage = workspace.DefaultLanguage
            };

            // Save datasources
            foreach (var datasource in workspace.Datasources.GetDatasources())
            {
                var definition = _datasourceFactory.CreateDefinition(datasource);
                if (definition != null)
                {
                    workspaceFile.Datasources.Add(definition);
                }
            }

            var json = JsonSerializer.Serialize(workspaceFile, JsonOptions);
            await File.WriteAllTextAsync(targetPath, json, cancellationToken);

            workspace.WorkspaceFilePath = targetPath;

            _logger.LogInformation("Saved workspace '{Name}'", workspace.Name);
        }

        /// <summary>
        /// Create a new workspace in a directory
        /// </summary>
        public async Task<WorkspaceArtifact> CreateNewAsync(string directory, string name = "Workspace", CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, WorkspaceFileExtension);
            // get default settings for new workspace
            var workspaceSettings = WorkspaceSettings.Instance;
            var workspace = new WorkspaceArtifact(name)
            {
                WorkspaceFilePath = filePath,
                RootNamespace = workspaceSettings.RootNamespace,
                DefaultOutputDirectory = workspaceSettings.DefaultOutputDirectory,
                DefaultTargetFramework = workspaceSettings.DefaultTargetFramework,
                DefaultLanguage = workspaceSettings.DefaultLanguage
            };

            await SaveAsync(workspace, filePath, cancellationToken);

            _logger.LogInformation("Created new workspace '{Name}' at {Directory}", name, directory);

            return workspace;
        }

        /// <summary>
        /// Check if a directory contains a workspace
        /// </summary>
        public bool IsWorkspaceDirectory(string directory)
        {
            var workspaceFile = Path.Combine(directory, WorkspaceFileExtension);
            return File.Exists(workspaceFile);
        }

        /// <summary>
        /// Get the workspace file path for a directory
        /// </summary>
        public string GetWorkspaceFilePath(string directory)
        {
            return Path.Combine(directory, WorkspaceFileExtension);
        }
    }
}
