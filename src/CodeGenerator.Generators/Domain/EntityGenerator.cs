using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators.Domain;

/// <summary>
/// Generates domain entity classes
/// </summary>
public class EntityGenerator : BaseCodeGenerator
{
    public override string Id => "Entity";
    public override string Name => "Entity Generator";
    public override string Description => "Generates domain entity classes";
    public override GeneratorType Type => GeneratorType.Entity;
    public override ArchitectureLayer Layer => ArchitectureLayer.Domain;

    public EntityGenerator(
        ITemplateEngine templateEngine,
        IFileService fileService,
        ILogger<EntityGenerator> logger)
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
                    Logger.LogInformation("Generated entity {EntityName} at {Path}", entity.Name, file.AbsolutePath);
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to generate entity {EntityName}", entity.Name);
            result.Errors.Add($"Failed to generate entity {entity.Name}: {ex.Message}");
            result.Success = false;
        }

        return result;
    }

    protected override object CreateTemplateModel(EntityModel entity, DomainContext context, GeneratorSettings settings)
    {
        return new
        {
            Entity = entity,
            Context = context,
            Settings = settings,
            Namespace = $"{settings.RootNamespace}.Domain.Entities",
            Usings = GetRequiredUsings(entity),
            Generator = new { Id, Name, GeneratedAt = DateTime.UtcNow }
        };
    }

    private List<string> GetRequiredUsings(EntityModel entity)
    {
        var usings = new List<string>
        {
            "System",
            "System.Collections.Generic",
            "System.ComponentModel.DataAnnotations"
        };

        if (entity.NavigationProperties.Any())
        {
            usings.Add("System.ComponentModel.DataAnnotations.Schema");
        }

        return usings.OrderBy(u => u).ToList();
    }
}
