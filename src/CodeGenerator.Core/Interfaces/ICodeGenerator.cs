using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;

namespace CodeGenerator.Core.Interfaces;

/// <summary>
/// Interface for code generators
/// </summary>
public interface ICodeGenerator
{
    /// <summary>
    /// Unique identifier for this generator
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Display name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description of what this generator produces
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Generator type
    /// </summary>
    GeneratorType Type { get; }

    /// <summary>
    /// Target architecture layer
    /// </summary>
    ArchitectureLayer Layer { get; }

    /// <summary>
    /// Supported target languages
    /// </summary>
    IReadOnlyList<TargetLanguage> SupportedLanguages { get; }

    /// <summary>
    /// Generate code for all entities in the context
    /// </summary>
    Task<GenerationResult> GenerateAsync(DomainContext context, GeneratorSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate code for a specific entity
    /// </summary>
    Task<GenerationResult> GenerateForEntityAsync(EntityModel entity, DomainContext context, GeneratorSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a preview of what will be generated
    /// </summary>
    Task<GenerationPreview> PreviewAsync(DomainContext context, GeneratorSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate the configuration for this generator
    /// </summary>
    ValidationResult Validate(GeneratorConfiguration configuration);
}