using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;

namespace CodeGenerator.Core.Interfaces;

/// <summary>
/// Interface for schema parsing
/// </summary>
public interface ISchemaParser
{
    /// <summary>
    /// Parse a JSON schema file into a domain context
    /// </summary>
    Task<DomainContext> ParseAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parse JSON schema content into a domain context
    /// </summary>
    Task<DomainContext> ParseContentAsync(string jsonContent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate a JSON schema file
    /// </summary>
    Task<ValidationResult> ValidateSchemaAsync(string filePath, CancellationToken cancellationToken = default);
}
