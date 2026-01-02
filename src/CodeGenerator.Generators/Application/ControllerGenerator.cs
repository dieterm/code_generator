using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators.Application;

/// <summary>
/// Generates controller classes
/// </summary>
public class ControllerGenerator : BaseCodeGenerator
{
    public override string Id => "Controller";
    public override string Name => "Controller Generator";
    public override string Description => "Generates controllers for the application layer";
    public override GeneratorType Type => GeneratorType.Controller;
    public override ArchitectureLayer Layer => ArchitectureLayer.Application;

    public ControllerGenerator(
        ITemplateEngine templateEngine,
        IFileService fileService,
        ILogger<ControllerGenerator> logger)
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

        // Skip owned types
        if (entity.IsOwnedType)
        {
            result.Success = true;
            return result;
        }

        // Check if controller generation is enabled for this entity
        if (entity.CodeGenSettings?.GenerateController == false)
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
                    Logger.LogInformation("Generated controller for {EntityName} at {Path}", entity.Name, file.AbsolutePath);
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to generate controller for {EntityName}", entity.Name);
            result.Errors.Add($"Failed to generate controller for {entity.Name}: {ex.Message}");
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
            Namespace = $"{settings.RootNamespace}.Application.Controllers",
            DomainNamespace = $"{settings.RootNamespace}.Domain.Entities",
            RepositoryNamespace = $"{settings.RootNamespace}.Application.Interfaces",
            ViewModelNamespace = $"{settings.RootNamespace}.Application.ViewModels",
            PrimaryKeyType = primaryKey?.TypeName ?? "int",
            PrimaryKeyName = primaryKey?.Name ?? "Id",
            RepositoryInterface = $"I{entity.Name}Repository",
            ViewModelName = $"{entity.Name}ViewModel",
            Generator = new { Id, Name, GeneratedAt = DateTime.UtcNow }
        };
    }
}
