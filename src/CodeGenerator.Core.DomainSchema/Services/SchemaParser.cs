using System.Text.Json;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Schema;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Services;

/// <summary>
/// Parses JSON Schema files into domain context
/// </summary>
public class SchemaParser
{
    private readonly ILogger<SchemaParser> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public SchemaParser(ILogger<SchemaParser> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            AllowTrailingCommas = true
        };
    }

    public async Task SaveSchemaAsync(DomainSchema schema, string filePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Saving schema to {FilePath}", filePath);
        var json = JsonSerializer.Serialize(schema, _jsonOptions);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    public async Task<DomainSchema> LoadSchemaAsync(string filePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Loading schema from {FilePath}", filePath);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Schema file not found: {filePath}");
        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        var schema = JsonSerializer.Deserialize<DomainSchema>(json, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to parse schema");
        foreach(var (name, definition) in schema.Definitions ?? new Dictionary<string, EntityDefinition>())
        {
            definition.Key = name;
            if (string.IsNullOrWhiteSpace(definition.Title))
            {
                definition.Title = name;
            }
        }

        return schema;
    }

    public async Task<ValidationResult> ValidateSchemaAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var result = new ValidationResult { IsValid = true };

        try
        {
            if (!File.Exists(filePath))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    Code = "FILE_NOT_FOUND",
                    Message = $"Schema file not found: {filePath}"
                });
                return result;
            }

            var schema = await LoadSchemaAsync(filePath, cancellationToken);
            //var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            //var schema = JsonSerializer.Deserialize<DomainSchema>(json, _jsonOptions);

            if (schema == null)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    Code = "INVALID_JSON",
                    Message = "Failed to parse JSON schema"
                });
                return result;
            }

            // Validate schema structure
            if (string.IsNullOrEmpty(schema.Schema))
            {
                result.Warnings.Add(new ValidationWarning
                {
                    Code = "MISSING_SCHEMA",
                    Message = "$schema property is recommended"
                });
            }

            if (schema.Definitions == null || !schema.Definitions.Any())
            {
                result.Warnings.Add(new ValidationWarning
                {
                    Code = "NO_DEFINITIONS",
                    Message = "No entity definitions found in $defs"
                });
            }

            // Validate each entity definition
            if (schema.Definitions != null)
            {
                foreach (var (name, definition) in schema.Definitions)
                {
                    ValidateEntityDefinition(name, definition, result);
                }
            }
        }
        catch (JsonException ex)
        {
            result.IsValid = false;
            result.Errors.Add(new ValidationError
            {
                Code = "JSON_PARSE_ERROR",
                Message = $"JSON parsing error: {ex.Message}",
                Path = ex.Path
            });
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add(new ValidationError
            {
                Code = "VALIDATION_ERROR",
                Message = $"Validation error: {ex.Message}"
            });
        }

        return result;
    }

    private void ValidateEntityDefinition(string name, EntityDefinition definition, ValidationResult result)
    {
        if (definition.Properties == null || !definition.Properties.Any())
        {
            result.Warnings.Add(new ValidationWarning
            {
                Code = "NO_PROPERTIES",
                Message = $"Entity '{name}' has no properties defined",
                Path = $"$defs/{name}"
            });
        }

        // Validate that required properties exist
        if (definition.Required != null && definition.Properties != null)
        {
            foreach (var required in definition.Required)
            {
                if (!definition.Properties.ContainsKey(required))
                {
                    result.Errors.Add(new ValidationError
                    {
                        Code = "MISSING_REQUIRED_PROPERTY",
                        Message = $"Required property '{required}' is not defined in entity '{name}'",
                        Path = $"$defs/{name}/required"
                    });
                    result.IsValid = false;
                }
            }
        }
    }
}
