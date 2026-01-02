using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators;

/// <summary>
/// Orchestrates code generation across all configured generators
/// </summary>
public class GeneratorOrchestrator
{
    private readonly IEnumerable<ICodeGenerator> _generators;
    private readonly ISchemaParser _schemaParser;
    private readonly IProjectGenerator _projectGenerator;
    private readonly ILogger<GeneratorOrchestrator> _logger;

    public GeneratorOrchestrator(
        IEnumerable<ICodeGenerator> generators,
        ISchemaParser schemaParser,
        IProjectGenerator projectGenerator,
        ILogger<GeneratorOrchestrator> logger)
    {
        _generators = generators;
        _schemaParser = schemaParser;
        _projectGenerator = projectGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Run all enabled generators
    /// </summary>
    public async Task<GenerationResult> GenerateAllAsync(
        GeneratorSettings settings,
        CancellationToken cancellationToken = default)
    {
        var result = new GenerationResult();
        var startTime = DateTime.UtcNow;

        try
        {
            _logger.LogInformation("Starting code generation from {SchemaPath}", settings.SchemaFilePath);

            // Parse schema
            var context = await _schemaParser.ParseAsync(settings.SchemaFilePath, cancellationToken);
            _logger.LogInformation("Parsed {EntityCount} entities from schema", context.Entities.Count);

            // Create projects if needed
            await CreateProjectsAsync(context, settings, result, cancellationToken);

            // Run generators in dependency order
            var orderedGenerators = OrderGeneratorsByDependency(settings);

            foreach (var generator in orderedGenerators)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var config = settings.Generators.GetValueOrDefault(generator.Id);
                if (config == null || !config.Enabled)
                    continue;

                _logger.LogInformation("Running generator: {GeneratorName}", generator.Name);

                var genResult = await generator.GenerateAsync(context, settings, cancellationToken);
                MergeResults(result, genResult);

                if (!genResult.Success)
                {
                    _logger.LogWarning("Generator {GeneratorName} completed with errors", generator.Name);
                }
            }

            result.Success = !result.Errors.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Code generation failed");
            result.Errors.Add($"Code generation failed: {ex.Message}");
            result.Success = false;
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

        var context = await _schemaParser.ParseAsync(settings.SchemaFilePath, cancellationToken);
        return await generator.GenerateAsync(context, settings, cancellationToken);
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
            var context = await _schemaParser.ParseAsync(settings.SchemaFilePath, cancellationToken);

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
}
