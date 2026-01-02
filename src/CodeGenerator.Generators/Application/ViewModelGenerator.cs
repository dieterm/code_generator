using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators.Application;

/// <summary>
/// Generates ViewModel classes
/// </summary>
public class ViewModelGenerator : BaseCodeGenerator
{
    public override string Id => "ViewModel";
    public override string Name => "ViewModel Generator";
    public override string Description => "Generates ViewModels for the application layer";
    public override GeneratorType Type => GeneratorType.ViewModel;
    public override ArchitectureLayer Layer => ArchitectureLayer.Application;

    public ViewModelGenerator(
        ITemplateEngine templateEngine,
        IFileService fileService,
        ILogger<ViewModelGenerator> logger)
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

        // Check if ViewModel generation is enabled for this entity
        if (entity.CodeGenSettings?.GenerateViewModel == false)
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
                    Logger.LogInformation("Generated ViewModel for {EntityName} at {Path}", entity.Name, file.AbsolutePath);
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to generate ViewModel for {EntityName}", entity.Name);
            result.Errors.Add($"Failed to generate ViewModel for {entity.Name}: {ex.Message}");
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
            Namespace = $"{settings.RootNamespace}.Application.ViewModels",
            DomainNamespace = $"{settings.RootNamespace}.Domain.Entities",
            RepositoryNamespace = $"{settings.RootNamespace}.Application.Interfaces",
            PrimaryKeyType = primaryKey?.TypeName ?? "int",
            PrimaryKeyName = primaryKey?.Name ?? "Id",
            Properties = GetViewModelProperties(entity),
            Generator = new { Id, Name, GeneratedAt = DateTime.UtcNow }
        };
    }

    private List<object> GetViewModelProperties(EntityModel entity)
    {
        return entity.Properties
            .Where(p => !p.IsComputed)
            .Select(p => new
            {
                p.Name,
                p.TypeName,
                p.IsNullable,
                p.IsRequired,
                p.Description,
                Display = p.DisplaySettings,
                Label = p.DisplaySettings?.Label ?? p.Name,
                IsVisible = p.DisplaySettings?.IsVisible ?? true,
                IsEditable = p.DisplaySettings?.IsEditable ?? true,
                ControlType = DetermineControlType(p),
                ValidationRules = p.ValidationRules
            })
            .Cast<object>()
            .ToList();
    }

    private string DetermineControlType(PropertyModel property)
    {
        if (property.DisplaySettings?.ControlType != null)
            return property.DisplaySettings.ControlType;

        return property.DataType switch
        {
            PropertyDataType.String when property.MaxLength > 500 => "TextArea",
            PropertyDataType.String => "TextBox",
            PropertyDataType.Integer or PropertyDataType.Long => "NumericUpDown",
            PropertyDataType.Decimal or PropertyDataType.Double or PropertyDataType.Float => "NumericUpDown",
            PropertyDataType.Boolean => "CheckBox",
            PropertyDataType.DateTime => "DateTimePicker",
            PropertyDataType.DateOnly => "DatePicker",
            PropertyDataType.TimeOnly => "TimePicker",
            PropertyDataType.Enum => "ComboBox",
            _ => "TextBox"
        };
    }
}
