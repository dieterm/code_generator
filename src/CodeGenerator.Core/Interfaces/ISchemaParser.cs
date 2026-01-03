using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Interfaces;

/// <summary>
/// Interface for schema parsing
/// </summary>
public interface ISchemaParser
{
    Task<DomainSchema> LoadSchemaAsync(string filePath, CancellationToken cancellationToken = default);
    /// <summary>
    /// Parse a JSON schema file into a domain context
    /// </summary>
    // Task<DomainContext> ParseFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parse JSON schema content into a domain context
    /// </summary>
    Task<DomainContext> ParseSchemaAsync(DomainSchema schema, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate a JSON schema file
    /// </summary>
    Task<ValidationResult> ValidateSchemaAsync(string filePath, CancellationToken cancellationToken = default);
}
