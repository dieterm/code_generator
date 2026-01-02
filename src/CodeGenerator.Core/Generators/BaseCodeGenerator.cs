using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Generators;

/// <summary>
/// Base class for all code generators
/// </summary>
public abstract class BaseCodeGenerator : ICodeGenerator
{
    protected readonly ITemplateEngine TemplateEngine;
    protected readonly IFileService FileService;
    protected readonly ILogger Logger;

    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract GeneratorType Type { get; }
    public abstract ArchitectureLayer Layer { get; }
    public virtual IReadOnlyList<TargetLanguage> SupportedLanguages => new[] { TargetLanguage.CSharp };

    protected BaseCodeGenerator(
        ITemplateEngine templateEngine,
        IFileService fileService,
        ILogger logger)
    {
        TemplateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        FileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual async Task<GenerationResult> GenerateAsync(
        DomainContext context,
        GeneratorSettings settings,
        CancellationToken cancellationToken = default)
    {
        var result = new GenerationResult();
        var startTime = DateTime.UtcNow;

        try
        {
            Logger.LogInformation("Starting generation with {GeneratorName} for {EntityCount} entities",
                Name, context.Entities.Count);

            var config = GetConfiguration(settings);
            if (config == null || !config.Enabled)
            {
                Logger.LogWarning("Generator {GeneratorId} is not configured or disabled", Id);
                result.Success = true;
                return result;
            }

            foreach (var entity in context.Entities)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var entityResult = await GenerateForEntityAsync(entity, context, settings, cancellationToken);
                result.Files.AddRange(entityResult.Files);
                result.Errors.AddRange(entityResult.Errors);
                result.Warnings.AddRange(entityResult.Warnings);
            }

            // Generate any shared files (e.g., base classes, interfaces)
            var sharedResult = await GenerateSharedFilesAsync(context, settings, cancellationToken);
            result.Files.AddRange(sharedResult.Files);

            result.Success = !result.Errors.Any();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during generation with {GeneratorName}", Name);
            result.Errors.Add($"Generation failed: {ex.Message}");
            result.Success = false;
        }
        finally
        {
            result.Duration = DateTime.UtcNow - startTime;
            result.GeneratedAt = DateTime.UtcNow;
        }

        return result;
    }

    public abstract Task<GenerationResult> GenerateForEntityAsync(
        EntityModel entity,
        DomainContext context,
        GeneratorSettings settings,
        CancellationToken cancellationToken = default);

    public virtual async Task<GenerationPreview> PreviewAsync(
        DomainContext context,
        GeneratorSettings settings,
        CancellationToken cancellationToken = default)
    {
        var preview = new GenerationPreview();
        var config = GetConfiguration(settings);

        if (config == null || !config.Enabled)
            return preview;

        foreach (var entity in context.Entities)
        {
            foreach (var template in config.Templates.Where(t => t.PerEntity))
            {
                var outputPath = ResolveOutputPath(template, entity, config, settings);
                var fileName = ResolveFileName(template, entity);

                var filePreview = new FilePreview
                {
                    RelativePath = outputPath,
                    FileName = fileName,
                    GeneratorId = Id,
                    EntityName = entity.Name,
                    WillOverwrite = FileService.FileExists(Path.Combine(settings.OutputFolder, outputPath))
                };

                // Generate preview content
                var templatePath = Path.Combine(settings.TemplateFolder, template.FileName);
                try
                {
                    var model = CreateTemplateModel(entity, context, settings);
                    filePreview.Content = await TemplateEngine.RenderFileAsync(templatePath, model, cancellationToken);
                }
                catch (Exception ex)
                {
                    filePreview.Content = $"//{templatePath}\r\n// Error generating preview: {ex.Message}";
                }

                if (filePreview.WillOverwrite)
                    preview.FilesToModify.Add(filePreview);
                else
                    preview.FilesToCreate.Add(filePreview);
            }
        }

        // Build folder structure
        preview.RootFolder = BuildFolderStructure(preview);

        return preview;
    }

    public virtual ValidationResult Validate(GeneratorConfiguration configuration)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrEmpty(configuration.OutputPathPattern))
        {
            result.Errors.Add(new ValidationError
            {
                Code = "MISSING_OUTPUT_PATH",
                Message = "Output path pattern is required"
            });
            result.IsValid = false;
        }

        if (!configuration.Templates.Any())
        {
            result.Warnings.Add(new ValidationWarning
            {
                Code = "NO_TEMPLATES",
                Message = "No templates configured for this generator"
            });
        }

        return result;
    }

    protected virtual async Task<GenerationResult> GenerateSharedFilesAsync(
        DomainContext context,
        GeneratorSettings settings,
        CancellationToken cancellationToken = default)
    {
        return new GenerationResult { Success = true };
    }

    protected GeneratorConfiguration? GetConfiguration(GeneratorSettings settings)
    {
        settings.Generators.TryGetValue(Id, out var config);
        return config;
    }

    protected virtual object CreateTemplateModel(EntityModel entity, DomainContext context, GeneratorSettings settings)
    {
        return new
        {
            Entity = entity,
            Context = context,
            Settings = settings,
            Generator = new
            {
                Id,
                Name,
                Type = Type.ToString(),
                Layer = Layer.ToString()
            },
            Helpers = new TemplateHelpers()
        };
    }

    protected virtual string ResolveOutputPath(TemplateReference template, EntityModel entity, GeneratorConfiguration config, GeneratorSettings settings)
    {
        var pattern = config.OutputPathPattern;
        var path = pattern
            .Replace("{Entity}", entity.Name)
            .Replace("{entity}", entity.Name.ToLowerInvariant())
            .Replace("{Layer}", Layer.ToString())
            .Replace("{layer}", Layer.ToString().ToLowerInvariant())
            .Replace("{Namespace}", entity.Namespace.Replace(".", Path.DirectorySeparatorChar.ToString()));

        // Normalize path separators to the current platform
        return NormalizePath(path);
    }

    protected virtual string ResolveFileName(TemplateReference template, EntityModel entity)
    {
        return template.OutputFileName
            .Replace("{Entity}", entity.Name)
            .Replace("{entity}", entity.Name.ToLowerInvariant());
    }

    /// <summary>
    /// Normalize path separators to the current platform
    /// </summary>
    protected static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        return path.Replace('/', Path.DirectorySeparatorChar)
                   .Replace('\\', Path.DirectorySeparatorChar);
    }

    protected FolderNode BuildFolderStructure(GenerationPreview preview)
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

    protected async Task<GeneratedFile> RenderAndCreateFile(
        TemplateReference template,
        EntityModel entity,
        DomainContext context,
        GeneratorSettings settings,
        CancellationToken cancellationToken)
    {
        var config = GetConfiguration(settings)!;
        var model = CreateTemplateModel(entity, context, settings);
        var templatePath = Path.Combine(settings.TemplateFolder, template.FileName);

        var content = await TemplateEngine.RenderFileAsync(templatePath, model, cancellationToken);

        var relativePath = ResolveOutputPath(template, entity, config, settings);
        var fileName = ResolveFileName(template, entity);
        var absolutePath = Path.Combine(settings.OutputFolder, relativePath, fileName);

        return new GeneratedFile
        {
            RelativePath = Path.Combine(relativePath, fileName),
            AbsolutePath = absolutePath,
            FileName = fileName,
            Content = content,
            GeneratorId = Id,
            TemplateId = template.Id,
            EntityName = entity.Name,
            IsNew = !FileService.FileExists(absolutePath)
        };
    }
}