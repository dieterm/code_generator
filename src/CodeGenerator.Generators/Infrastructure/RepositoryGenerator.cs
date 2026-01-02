using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators.Infrastructure;

/// <summary>
/// Generates repository interfaces and implementations
/// </summary>
public class RepositoryGenerator : BaseCodeGenerator
{
    public override string Id => "Repository";
    public override string Name => "Repository Generator";
    public override string Description => "Generates repository interfaces and implementations";
    public override GeneratorType Type => GeneratorType.Repository;
    public override ArchitectureLayer Layer => ArchitectureLayer.Infrastructure;

    public RepositoryGenerator(
        ITemplateEngine templateEngine,
        IFileService fileService,
        ILogger<RepositoryGenerator> logger)
        : base(templateEngine, fileService, logger)
    {
    }

    public override async Task<GenerationResult> GenerateForEntityAsync(
        EntityModel entity,
        DomainContext context,
        GeneratorSettings settings,
        CancellationToken cancellationToken = default)
    {
        var result = new GenerationResult();
        var config = GetConfiguration(settings);

        if (config == null || !config.Enabled)
        {
            result.Success = true;
            return result;
        }

        // Skip owned types - they don't need repositories
        if (entity.IsOwnedType)
        {
            result.Success = true;
            return result;
        }

        // Check if repository generation is enabled for this entity
        if (entity.CodeGenSettings?.GenerateRepository == false)
        {
            result.Success = true;
            return result;
        }

        try
        {
            foreach (var template in config.Templates.Where(t => t.PerEntity))
            {
                var file = await RenderAndCreateFile(template, entity, context, settings, cancellationToken);
                result.Files.Add(file);

                if (settings.OverwriteExisting || file.IsNew)
                {
                    await FileService.WriteFileAsync(file.AbsolutePath, file.Content, settings.CreateBackup, cancellationToken);
                    file.Written = true;
                    Logger.LogInformation("Generated repository for {EntityName} at {Path}", entity.Name, file.AbsolutePath);
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to generate repository for {EntityName}", entity.Name);
            result.Errors.Add($"Failed to generate repository for {entity.Name}: {ex.Message}");
            result.Success = false;
        }

        return result;
    }

    protected override object CreateTemplateModel(EntityModel entity, DomainContext context, GeneratorSettings settings)
    {
        var primaryKey = entity.PrimaryKeyProperties.FirstOrDefault();

        return new
        {
            Entity = entity,
            Context = context,
            Settings = settings,
            Namespace = $"{settings.RootNamespace}.Infrastructure.Repositories",
            InterfaceNamespace = $"{settings.RootNamespace}.Application.Interfaces",
            DomainNamespace = $"{settings.RootNamespace}.Domain.Entities",
            PrimaryKeyType = primaryKey?.TypeName ?? "int",
            PrimaryKeyName = primaryKey?.Name ?? "Id",
            DbContextName = $"{context.Name}DbContext",
            Generator = new { Id, Name, GeneratedAt = DateTime.UtcNow }
        };
    }

    protected override async Task<GenerationResult> GenerateSharedFilesAsync(
        DomainContext context,
        GeneratorSettings settings,
        CancellationToken cancellationToken = default)
    {
        var result = new GenerationResult();
        var config = GetConfiguration(settings);

        if (config == null || !config.Enabled)
        {
            result.Success = true;
            return result;
        }

        // Generate base repository interface and implementation
        try
        {
            var baseRepoTemplate = config.Templates.FirstOrDefault(t => t.Id == "repository-base");
            if (baseRepoTemplate != null)
            {
                var model = new
                {
                    Namespace = $"{settings.RootNamespace}.Infrastructure.Repositories",
                    InterfaceNamespace = $"{settings.RootNamespace}.Application.Interfaces",
                    DomainNamespace = $"{settings.RootNamespace}.Domain.Entities",
                    DbContextName = $"{context.Name}DbContext",
                    Settings = settings,
                    Generator = new { Id, Name, GeneratedAt = DateTime.UtcNow }
                };

                var templatePath = Path.Combine(settings.TemplateFolder, baseRepoTemplate.FileName);
                var content = await TemplateEngine.RenderFileAsync(templatePath, model, cancellationToken);

                var outputPath = config.OutputPathPattern.Replace("{Layer}", Layer.ToString());
                var fileName = baseRepoTemplate.OutputFileName;
                var absolutePath = Path.Combine(settings.OutputFolder, outputPath, fileName);

                FileService.EnsureDirectory(Path.GetDirectoryName(absolutePath)!);
                await FileService.WriteFileAsync(absolutePath, content, settings.CreateBackup, cancellationToken);

                result.Files.Add(new GeneratedFile
                {
                    RelativePath = Path.Combine(outputPath, fileName),
                    AbsolutePath = absolutePath,
                    FileName = fileName,
                    Content = content,
                    GeneratorId = Id,
                    TemplateId = baseRepoTemplate.Id,
                    Written = true
                });
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to generate base repository");
            result.Errors.Add($"Failed to generate base repository: {ex.Message}");
        }

        return result;
    }
}
