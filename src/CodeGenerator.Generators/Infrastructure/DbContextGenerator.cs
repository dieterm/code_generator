using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators.Infrastructure;

/// <summary>
/// Generates Entity Framework Core DbContext
/// </summary>
public class DbContextGenerator : BaseCodeGenerator
{
    public override string Id => "DbContext";
    public override string Name => "DbContext Generator";
    public override string Description => "Generates Entity Framework Core DbContext";
    public override GeneratorType Type => GeneratorType.DbContext;
    public override ArchitectureLayer Layer => ArchitectureLayer.Infrastructure;

    public DbContextGenerator(
        ITemplateEngine templateEngine,
        IFileService fileService,
        ILogger<DbContextGenerator> logger)
        : base(templateEngine, fileService, logger)
    {
    }

    public override Task<GenerationResult> GenerateForEntityAsync(
        EntityModel entity,
        DomainContext context,
        GeneratorSettings settings,
        CancellationToken cancellationToken = default)
    {
        // DbContext is generated once for all entities, not per entity
        return Task.FromResult(new GenerationResult { Success = true });
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

        try
        {
            var template = config.Templates.FirstOrDefault(t => !t.PerEntity);
            if (template == null)
            {
                result.Warnings.Add("No DbContext template configured");
                result.Success = true;
                return result;
            }

            var model = CreateDbContextModel(context, settings);
            var templatePath = Path.Combine(settings.TemplateFolder, template.FileName);
            var content = await TemplateEngine.RenderFileAsync(templatePath, model, cancellationToken);

            var outputPath = config.OutputPathPattern.Replace("{Layer}", Layer.ToString());
            var fileName = template.OutputFileName;
            var absolutePath = Path.Combine(settings.OutputFolder, outputPath, fileName);

            var file = new GeneratedFile
            {
                RelativePath = Path.Combine(outputPath, fileName),
                AbsolutePath = absolutePath,
                FileName = fileName,
                Content = content,
                GeneratorId = Id,
                TemplateId = template.Id,
                IsNew = !FileService.FileExists(absolutePath)
            };

            if (settings.OverwriteExisting || file.IsNew)
            {
                FileService.EnsureDirectory(Path.GetDirectoryName(absolutePath)!);
                await FileService.WriteFileAsync(absolutePath, content, settings.CreateBackup, cancellationToken);
                file.Written = true;
                Logger.LogInformation("Generated DbContext at {Path}", absolutePath);
            }

            result.Files.Add(file);
            result.Success = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to generate DbContext");
            result.Errors.Add($"Failed to generate DbContext: {ex.Message}");
            result.Success = false;
        }

        return result;
    }

    private object CreateDbContextModel(DomainContext context, GeneratorSettings settings)
    {
        var dbName = context.DatabaseMetadata?.DatabaseName ?? context.Name;
        var contextName = $"{dbName}DbContext";

        return new
        {
            ContextName = contextName,
            Namespace = $"{settings.RootNamespace}.Infrastructure.Data",
            DomainNamespace = $"{settings.RootNamespace}.Domain.Entities",
            Entities = context.Entities.Where(e => !e.IsOwnedType).ToList(),
            OwnedEntities = context.Entities.Where(e => e.IsOwnedType).ToList(),
            DatabaseMetadata = context.DatabaseMetadata,
            Context = context,
            Settings = settings,
            Generator = new { Id, Name, GeneratedAt = DateTime.UtcNow }
        };
    }
}
