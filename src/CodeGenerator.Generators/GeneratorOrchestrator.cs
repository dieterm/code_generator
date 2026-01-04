using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Events;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators;

/// <summary>
/// Orchestrates code generation across all configured generators
/// </summary>
public class GeneratorOrchestrator
{
    private readonly IEnumerable<ICodeGenerator> _generators;
    private readonly IEnumerable<IMessageBusAwareGenerator> _messageBusAwareGenerators;
    private readonly ISchemaParser _schemaParser;
    private readonly IProjectGenerator _projectGenerator;
    private readonly IGeneratorMessageBus _messageBus;
    private readonly ILogger<GeneratorOrchestrator> _logger;

    public GeneratorOrchestrator(
        IEnumerable<ICodeGenerator> generators,
        IEnumerable<IMessageBusAwareGenerator> messageBusAwareGenerators,
        ISchemaParser schemaParser,
        IProjectGenerator projectGenerator,
        IGeneratorMessageBus messageBus,
        ILogger<GeneratorOrchestrator> logger)
    {
        _generators = generators;
        _schemaParser = schemaParser;
        _projectGenerator = projectGenerator;
        _messageBus = messageBus;
        _logger = logger;
        _messageBusAwareGenerators = messageBusAwareGenerators;

        InitializeGenerators();
    }

    /// <summary>
    /// The message bus for event communication
    /// </summary>
    public IGeneratorMessageBus MessageBus => _messageBus;

    private void InitializeGenerators()
    {
        foreach (var generator in _messageBusAwareGenerators)
        {
            generator.Initialize(_messageBus);
            generator.SubscribeToEvents(_messageBus);
            _logger.LogDebug("Initialized message bus aware generator: {GeneratorName}", generator.Name);
        }
    }

    /// <summary>
    /// Run all enabled generators
    /// </summary>
    public async Task<GenerationResult> GenerateAllAsync(
        GeneratorSettings settings,
        IProgress<GenerationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new GenerationResult();
        var startTime = DateTime.UtcNow;

        try
        {
            // Phase: Initializing
            ReportProgress(progress, GenerationPhase.Initializing, "Loading schema...", 0);
            _logger.LogInformation("Starting code generation from {SchemaPath}", settings.SchemaFilePath);

            // Parse schema
            var schema = await _schemaParser.LoadSchemaAsync(settings.SchemaFilePath, cancellationToken);
            ReportProgress(progress, GenerationPhase.Initializing, "Parsing schema...", 5);
            
            var context = await _schemaParser.ParseSchemaAsync(schema, cancellationToken);
            _logger.LogInformation("Parsed {EntityCount} entities from schema", context.Entities.Count);
            ReportProgress(progress, GenerationPhase.Initializing, $"Parsed {context.Entities.Count} entities", 10);

            // Create solution and projects
            ReportProgress(progress, GenerationPhase.CreatingSolution, "Creating solution and projects...", 15);
            await CreateSolutionAndProjectsAsync(schema, context, settings, result, progress, cancellationToken);

            // Run generators in dependency order
            var orderedGenerators = OrderGeneratorsByDependency(settings);
            var enabledGenerators = orderedGenerators
                .Where(g => settings.Generators.GetValueOrDefault(g.Id)?.Enabled == true)
                .ToList();

            if (enabledGenerators.Count > 0)
            {
                ReportProgress(progress, GenerationPhase.RunningGenerators, "Running generators...", 70);
                
                int generatorIndex = 0;
                foreach (var generator in enabledGenerators)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var percentInGeneratorPhase = 70 + (int)((generatorIndex / (float)enabledGenerators.Count) * 25);
                    ReportProgress(progress, GenerationPhase.RunningGenerators, 
                        $"Running generator: {generator.Name}", percentInGeneratorPhase,
                        enabledGenerators.Count, generatorIndex);

                    _logger.LogInformation("Running generator: {GeneratorName}", generator.Name);

                    var genResult = await generator.GenerateAsync(context, settings, cancellationToken);
                    
                    // Publish events for each generated file
                    foreach (var file in genResult.Files)
                    {
                        var createdFileEvent = new CreatedFileEventArgs(schema, context, file);
                        await _messageBus.PublishAsync(createdFileEvent, cancellationToken);
                    }
                    
                    MergeResults(result, genResult);

                    if (!genResult.Success)
                    {
                        _logger.LogWarning("Generator {GeneratorName} completed with errors", generator.Name);
                    }

                    generatorIndex++;
                }
            }

            // Phase: Finalizing
            ReportProgress(progress, GenerationPhase.Finalizing, "Finalizing...", 95);
            result.Success = !result.Errors.Any();

            // Phase: Completed
            ReportProgress(progress, GenerationPhase.Completed, 
                $"Completed: {result.Files.Count} files generated", 100);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Code generation failed");
            result.Errors.Add($"Code generation failed: {ex.Message}");
            result.Success = false;
            ReportProgress(progress, GenerationPhase.Completed, $"Failed: {ex.Message}", 100);
        }
        finally
        {
            result.Duration = DateTime.UtcNow - startTime;
            result.GeneratedAt = DateTime.UtcNow;
        }

        _logger.LogInformation(
            "Code generation completed in {Duration}. Files: {FileCount}, Errors: {ErrorCount}",
            result.Duration, result.Files.Count, result.Errors.Count);

        return result;
    }

    /// <summary>
    /// Run a specific generator by ID
    /// </summary>
    public async Task<GenerationResult> GenerateAsync(
        string generatorId,
        GeneratorSettings settings,
        IProgress<GenerationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var generator = _generators.FirstOrDefault(g => g.Id.Equals(generatorId, StringComparison.OrdinalIgnoreCase));
        if (generator == null)
        {
            return new GenerationResult
            {
                Success = false,
                Errors = { $"Generator '{generatorId}' not found" }
            };
        }

        ReportProgress(progress, GenerationPhase.Initializing, "Loading schema...", 0);
        var schema = await _schemaParser.LoadSchemaAsync(settings.SchemaFilePath, cancellationToken);
        
        ReportProgress(progress, GenerationPhase.Initializing, "Parsing schema...", 20);
        var context = await _schemaParser.ParseSchemaAsync(schema, cancellationToken);
        
        ReportProgress(progress, GenerationPhase.RunningGenerators, $"Running {generator.Name}...", 40);
        var result = await generator.GenerateAsync(context, settings, cancellationToken);
        
        ReportProgress(progress, GenerationPhase.Completed, 
            $"Completed: {result.Files.Count} files generated", 100);
        
        return result;
    }

    private void ReportProgress(
        IProgress<GenerationProgress>? progress, 
        GenerationPhase phase, 
        string message, 
        int percentComplete,
        int totalItems = 0,
        int completedItems = 0)
    {
        progress?.Report(new GenerationProgress
        {
            Phase = phase,
            CurrentStep = phase.ToString(),
            Message = message,
            PercentComplete = percentComplete,
            TotalItems = totalItems,
            CompletedItems = completedItems,
            IsIndeterminate = false
        });
    }

    private void RegisterDefaultProjects(CreatingSolutionEventArgs eventArgs, ProjectSettings projectSettings, GeneratorSettings settings)
    {
        var layers = new Dictionary<ArchitectureLayer, (string? Name, string Type)>
        {
            [ArchitectureLayer.Domain] = (projectSettings.DomainProjectName, "classlib"),
            [ArchitectureLayer.Application] = (projectSettings.ApplicationProjectName, "classlib"),
            [ArchitectureLayer.Infrastructure] = (projectSettings.InfrastructureProjectName, "classlib"),
            [ArchitectureLayer.Presentation] = (projectSettings.PresentationProjectName, "winformslib")
        };
        var solutionNamespace = eventArgs.Schema?.CodeGenMetadata?.Namespace;

        foreach (var (layer, (projectName, projectType)) in layers)
        {
            if (string.IsNullOrEmpty(projectName)) continue;
            var projectFullName = projectName;
            if (!string.IsNullOrWhiteSpace(solutionNamespace))
                projectFullName = $"{solutionNamespace}.{projectName}";
            var registration = new ProjectRegistration
            {
                Layer = layer,
                ProjectName = projectFullName,
                ProjectPath = Path.Combine(settings.OutputFolder, projectFullName),
                ProjectType = projectType,
                TargetFramework = settings.TargetFramework,
                RegisteredBy = "GeneratorOrchestrator"
            };
            
            // Add NuGet packages for this layer
            var packages = settings.NuGetPackages
                .Where(p => p.Layers.Contains(layer))
                .Select(p => new NuGetPackageInfo { PackageId = p.PackageId, Version = p.Version });
            registration.NuGetPackages.AddRange(packages);

            eventArgs.ProjectRegistrations.Add(registration);
        }
    }

    /// <summary>
    /// Generate a preview of all changes
    /// </summary>
    public async Task<GenerationPreview> PreviewAllAsync(
        GeneratorSettings settings,
        CancellationToken cancellationToken = default)
    {
        var preview = new GenerationPreview();

        try
        {
            var schema = await _schemaParser.LoadSchemaAsync(settings.SchemaFilePath, cancellationToken);
            var context = await _schemaParser.ParseSchemaAsync(schema, cancellationToken);

            foreach (var generator in _generators)
            {
                var config = settings.Generators.GetValueOrDefault(generator.Id);
                if (config == null || !config.Enabled)
                    continue;

                var genPreview = await generator.PreviewAsync(context, settings, cancellationToken);
                preview.FilesToCreate.AddRange(genPreview.FilesToCreate);
                preview.FilesToModify.AddRange(genPreview.FilesToModify);
                preview.ProjectsToCreate.AddRange(genPreview.ProjectsToCreate);
            }

            preview.RootFolder = BuildFolderTree(preview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate preview");
        }

        return preview;
    }

    /// <summary>
    /// Get list of available generators
    /// </summary>
    public IReadOnlyList<GeneratorInfo> GetAvailableGenerators()
    {
        return _generators.Select(g => new GeneratorInfo
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Type = g.Type,
            Layer = g.Layer,
            SupportedLanguages = g.SupportedLanguages.ToList()
        }).ToList();
    }

    private async Task CreateProjectsAsync(
        DomainContext context,
        GeneratorSettings settings,
        GenerationResult result,
        CancellationToken cancellationToken)
    {
        var projectSettings = context.CodeGenMetadata?.ProjectSettings;
        if (projectSettings == null) return;

        var layers = new Dictionary<ArchitectureLayer, string?>
        {
            [ArchitectureLayer.Domain] = projectSettings.DomainProjectName,
            [ArchitectureLayer.Application] = projectSettings.ApplicationProjectName,
            [ArchitectureLayer.Infrastructure] = projectSettings.InfrastructureProjectName,
            [ArchitectureLayer.Presentation] = projectSettings.PresentationProjectName
        };

        foreach (var (layer, projectName) in layers.Where(l => !string.IsNullOrEmpty(l.Value)))
        {
            try
            {
                var projectType = layer == ArchitectureLayer.Presentation ? "winformslib" : "classlib";
                var projectDir = Path.Combine(settings.OutputFolder, projectName!);

                var project = await _projectGenerator.CreateProjectAsync(
                    projectName!, projectDir, projectType, settings.TargetFramework, cancellationToken);

                // Add NuGet packages for this layer
                var packages = settings.NuGetPackages.Where(p => p.Layers.Contains(layer));
                foreach (var package in packages)
                {
                    await _projectGenerator.AddPackageAsync(project.ProjectFilePath, package.PackageId, package.Version, cancellationToken);
                }

                result.Projects.Add(project);
                result.Messages.Add($"Created project: {projectName}");
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Failed to create project {projectName}: {ex.Message}");
            }
        }
    }

    private IEnumerable<ICodeGenerator> OrderGeneratorsByDependency(GeneratorSettings settings)
    {
        var ordered = new List<ICodeGenerator>();
        var visited = new HashSet<string>();

        void Visit(ICodeGenerator generator)
        {
            if (visited.Contains(generator.Id))
                return;

            visited.Add(generator.Id);

            var config = settings.Generators.GetValueOrDefault(generator.Id);
            if (config?.DependsOn != null)
            {
                foreach (var depId in config.DependsOn)
                {
                    var dep = _generators.FirstOrDefault(g => g.Id == depId);
                    if (dep != null)
                        Visit(dep);
                }
            }

            ordered.Add(generator);
        }

        foreach (var generator in _generators)
        {
            Visit(generator);
        }

        return ordered;
    }

    private void MergeResults(GenerationResult target, GenerationResult source)
    {
        target.Files.AddRange(source.Files);
        target.Projects.AddRange(source.Projects);
        target.Errors.AddRange(source.Errors);
        target.Warnings.AddRange(source.Warnings);
        target.Messages.AddRange(source.Messages);
    }

    private FolderNode BuildFolderTree(GenerationPreview preview)
    {
        var root = new FolderNode { Name = "Output", FullPath = "" };
        var allFiles = preview.FilesToCreate.Concat(preview.FilesToModify).ToList();

        foreach (var file in allFiles)
        {
            // Combine RelativePath with FileName to get full path
            var fullPath = string.IsNullOrEmpty(file.RelativePath) 
                ? file.FileName 
                : Path.Combine(file.RelativePath, file.FileName);

            // Normalize path separators to the current platform
            fullPath = NormalizePath(fullPath);

            var parts = fullPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            var current = root;

            // Navigate/create folder structure (all parts except the last, which is the filename)
            for (int i = 0; i < parts.Length - 1; i++)
            {
                var folder = current.Folders.FirstOrDefault(f => f.Name == parts[i]);
                if (folder == null)
                {
                    folder = new FolderNode
                    {
                        Name = parts[i],
                        FullPath = string.Join(Path.DirectorySeparatorChar.ToString(), parts.Take(i + 1))
                    };
                    current.Folders.Add(folder);
                }
                current = folder;
            }

            // Add file to the deepest folder
            current.Files.Add(file);
        }

        return root;
    }

    private static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        return path.Replace('/', Path.DirectorySeparatorChar)
                   .Replace('\\', Path.DirectorySeparatorChar);
    }

    private async Task CreateSolutionAndProjectsAsync(
        DomainSchema schema,
        DomainContext context,
        GeneratorSettings settings,
        GenerationResult result,
        IProgress<GenerationProgress>? progress,
        CancellationToken cancellationToken)
    {
        var projectSettings = context.CodeGenMetadata?.ProjectSettings;
        if (projectSettings == null) return;

        // Create solution info
        var solutionInfo = new SolutionInfo
        {
            SolutionName = projectSettings.SolutionName ?? "GeneratedSolution",
            SolutionPath = settings.OutputFolder,
            TargetFramework = settings.TargetFramework
        };

        // Publish CreatingSolution event - generators can register projects
        var creatingSolutionEvent = new CreatingSolutionEventArgs(schema, context, solutionInfo, settings);
        
        await _messageBus.PublishAsync(creatingSolutionEvent, cancellationToken);
        
        _logger.LogInformation("CreatingSolution event published. {ProjectCount} projects registered.", 
            creatingSolutionEvent.ProjectRegistrations.Count);

        var createdProjects = new List<GeneratedProject>();
        var projectRegistrations = new Dictionary<ProjectRegistration, (CreatingProjectEventArgs args, GeneratedProject project)>();
        
        var totalProjects = creatingSolutionEvent.ProjectRegistrations.Count;
        var projectIndex = 0;

        // Create each registered project
        foreach (var projectReg in creatingSolutionEvent.ProjectRegistrations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var projectPercent = 15 + (int)((projectIndex / (float)Math.Max(totalProjects, 1)) * 40);
            ReportProgress(progress, GenerationPhase.CreatingProjects, 
                $"Creating project: {projectReg.ProjectName}", projectPercent,
                totalProjects, projectIndex);

            try
            {
                // Publish CreatingProject event - generators can register files
                var creatingProjectEvent = new CreatingProjectEventArgs(schema, context, projectReg);
                await _messageBus.PublishAsync(creatingProjectEvent, cancellationToken);
                
                _logger.LogInformation("CreatingProject event for {ProjectName}. {FileCount} files, {PackageCount} packages registered.",
                    projectReg.ProjectName, 
                    creatingProjectEvent.FileRegistrations.Count,
                    creatingProjectEvent.AdditionalPackages.Count);

                
                // Create the project
                var projectDir = Path.Combine(settings.OutputFolder, projectReg.ProjectName);
                var project = await _projectGenerator.CreateProjectAsync(
                    projectReg.ProjectName, 
                    projectDir, 
                    projectReg.ProjectType, 
                    projectReg.TargetFramework, 
                    cancellationToken);

                // save event args for creating project references once all projects are generated
                projectRegistrations.Add(projectReg, (creatingProjectEvent, project));

                // Add NuGet packages
                var allPackages = projectReg.NuGetPackages.Concat(creatingProjectEvent.AdditionalPackages).ToList();
                if (allPackages.Count > 0)
                {
                    ReportProgress(progress, GenerationPhase.AddingPackages, 
                        $"Adding packages to {projectReg.ProjectName}", projectPercent,
                        allPackages.Count, 0);
                        
                    foreach (var package in allPackages)
                    {
                        await _projectGenerator.AddPackageAsync(project.ProjectFilePath, package.PackageId, package.Version, cancellationToken);
                    }
                }
                
                // Create registered files
                var createdFiles = new List<GeneratedFile>();
                var totalFiles = creatingProjectEvent.FileRegistrations.Count;
                var fileIndex = 0;

                foreach (var fileReg in creatingProjectEvent.FileRegistrations)
                {
                    if (totalFiles > 0 && fileIndex % 10 == 0) // Report every 10 files to avoid too many updates
                    {
                        ReportProgress(progress, GenerationPhase.GeneratingFiles, 
                            $"Generating files for {projectReg.ProjectName} ({fileIndex}/{totalFiles})", 
                            projectPercent, totalFiles, fileIndex);
                    }

                    var creatingFileEvent = new CreatingFileEventArgs(schema, context, fileReg, project, fileReg.TemplateName);
                    await _messageBus.PublishAsync(creatingFileEvent, cancellationToken);
                    
                    if (creatingFileEvent.Cancel)
                    {
                        _logger.LogDebug("File creation cancelled: {FileName}. Reason: {Reason}", 
                            fileReg.FileName, creatingFileEvent.CancelReason);
                        fileIndex++;
                        continue;
                    }
                    var filePath = Path.Combine(projectDir, fileReg.RelativePath, fileReg.FileName);

                    if (File.Exists(filePath) && !settings.OverwriteExisting){
                        _logger.LogDebug("Skipped File. File already exists and overwrite is disabled in settings: {FilePath}", filePath);
                        fileIndex++;
                        continue;
                    }
                    
                    var fileDir = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(fileDir) && !Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }

                    await File.WriteAllTextAsync(filePath, fileReg.Content, cancellationToken);
                    
                    var generatedFile = new GeneratedFile
                    {
                        FileName = fileReg.FileName,
                        RelativePath = fileReg.RelativePath,
                        AbsolutePath = filePath,
                        Content = fileReg.Content                        
                    };
                    createdFiles.Add(generatedFile);
                    result.Files.Add(generatedFile);
                    
                    await _messageBus.PublishAsync(new CreatedFileEventArgs(schema, context, generatedFile, project), cancellationToken);
                    fileIndex++;
                }

                createdProjects.Add(project);
                result.Projects.Add(project);
                result.Messages.Add($"Created project: {projectReg.ProjectName}");

                // Publish CreatedProject event
                var createdProjectEvent = new CreatedProjectEventArgs(schema, context, project, createdFiles);
                await _messageBus.PublishAsync(createdProjectEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Failed to create project {projectReg.ProjectName}: {ex.Message}");
                _logger.LogError(ex, "Failed to create project {ProjectName}", projectReg.ProjectName);
            }

            projectIndex++;
        }

        // Add project references
        ReportProgress(progress, GenerationPhase.AddingReferences, "Adding project references...", 58);
        
        foreach(var projectReg in creatingSolutionEvent.ProjectRegistrations)
        {
            (CreatingProjectEventArgs creatingProjectEvent, GeneratedProject project) = projectRegistrations[projectReg];
            var allProjectRefs = projectReg.ProjectReferences.Concat(creatingProjectEvent.AdditionalProjectReferences);

            foreach (var projRef in allProjectRefs)
            {
                var refProject = createdProjects.FirstOrDefault(p => p.Name.Equals(projRef, StringComparison.OrdinalIgnoreCase));
                if (refProject != null)
                {
                    await _projectGenerator.AddProjectReferenceAsync(project.ProjectFilePath, refProject.ProjectFilePath, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Project reference {ProjectReference} not found for project {ProjectName}.", projRef, projectReg.ProjectName);
                }
            }
        }

        // Create the .sln solution file
        ReportProgress(progress, GenerationPhase.CreatingSolution, "Creating solution file...", 65);
        await _projectGenerator.CreateSolutionAsync(solutionInfo.SolutionName, solutionInfo.SolutionPath, createdProjects.Select(p => p.Directory), cancellationToken);
        
        // Publish CreatedSolution event
        var createdSolutionEvent = new CreatedSolutionEventArgs(schema, context, solutionInfo, createdProjects);
        await _messageBus.PublishAsync(createdSolutionEvent, cancellationToken);
    }
}
