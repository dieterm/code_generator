using System.Text.Json;
using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Schema;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Services;

/// <summary>
/// Parses JSON Schema files into domain context
/// </summary>
public class SchemaParser : ISchemaParser
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
            AllowTrailingCommas = true
        };
    }

    public async Task<DomainContext> ParseAsync(string filePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Parsing schema from {FilePath}", filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Schema file not found: {filePath}");

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        return await ParseContentAsync(json, cancellationToken);
    }

    public async Task<DomainContext> ParseContentAsync(string jsonContent, CancellationToken cancellationToken = default)
    {
        var schema = JsonSerializer.Deserialize<DomainSchema>(jsonContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to parse schema");

        var context = new DomainContext
        {
            Name = schema.Title ?? "DomainContext",
            Description = schema.Description,
            Namespace = schema.CodeGenMetadata?.Namespace ?? "Generated",
            CodeGenMetadata = schema.CodeGenMetadata,
            DatabaseMetadata = schema.DatabaseMetadata,
            OriginalSchema = schema
        };

        // Parse definitions ($defs) as entities
        if (schema.Definitions != null)
        {
            foreach (var (name, definition) in schema.Definitions)
            {
                var entity = ParseEntity(name, definition, context.Namespace);
                context.Entities.Add(entity);
            }
        }

        // Parse root properties as additional entities or enums
        if (schema.Properties != null)
        {
            foreach (var (name, property) in schema.Properties)
            {
                if (property.Type == "object" && property.Ref == null)
                {
                    // This could be an inline entity definition
                    // For now, we'll handle references
                }
                else if (property.Enum != null)
                {
                    var enumModel = ParseEnum(name, property);
                    context.Enums.Add(enumModel);
                }
            }
        }

        // Resolve relationships between entities
        ResolveRelationships(context);

        await Task.CompletedTask;
        return context;
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

            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            var schema = JsonSerializer.Deserialize<DomainSchema>(json, _jsonOptions);

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

    private EntityModel ParseEntity(string name, EntityDefinition definition, string defaultNamespace)
    {
        var entity = new EntityModel
        {
            Name = definition.CodeGenMetadata?.ClassName ?? name,
            Namespace = definition.CodeGenMetadata?.Namespace ?? defaultNamespace,
            Description = definition.Description ?? definition.Title,
            BaseClass = definition.CodeGenMetadata?.BaseClass,
            Interfaces = definition.CodeGenMetadata?.Interfaces ?? new List<string>(),
            IsAbstract = definition.CodeGenMetadata?.IsAbstract ?? false,
            IsSealed = definition.CodeGenMetadata?.IsSealed ?? false,
            IsOwnedType = definition.CodeGenMetadata?.IsOwnedType ?? false,
            CodeGenSettings = definition.CodeGenMetadata,
            DatabaseSettings = definition.DatabaseMetadata,
            OriginalDefinition = definition
        };

        entity.FullName = $"{entity.Namespace}.{entity.Name}";

        // Parse properties
        if (definition.Properties != null)
        {
            foreach (var (propName, propDef) in definition.Properties)
            {
                var property = ParseProperty(propName, propDef, definition.Required);
                entity.Properties.Add(property);

                if (property.IsPrimaryKey)
                    entity.PrimaryKeyProperties.Add(property);
            }
        }

        // Parse custom attributes
        if (definition.CodeGenMetadata?.CustomAttributes != null)
        {
            foreach (var attr in definition.CodeGenMetadata.CustomAttributes)
            {
                entity.Attributes.Add(new AttributeModel
                {
                    Name = attr.Name,
                    NamedArguments = attr.Arguments ?? new Dictionary<string, object>()
                });
            }
        }

        return entity;
    }

    private PropertyModel ParseProperty(string name, PropertyDefinition definition, List<string>? required)
    {
        var property = new PropertyModel
        {
            Name = definition.CodeGenMetadata?.PropertyName ?? name,
            Description = definition.Description ?? definition.Title,
            IsRequired = required?.Contains(name) ?? false,
            IsNullable = definition.CodeGenMetadata?.IsNullable ?? !(required?.Contains(name) ?? false),
            DefaultValue = definition.Default,
            MaxLength = definition.MaxLength,
            MinLength = definition.MinLength,
            Minimum = definition.Minimum,
            Maximum = definition.Maximum,
            Pattern = definition.Pattern,
            IsPrimaryKey = definition.DatabaseMetadata?.IsPrimaryKey ?? false,
            IsForeignKey = definition.DatabaseMetadata?.IsForeignKey ?? false,
            IsAutoGenerated = definition.DatabaseMetadata?.IsIdentity ?? false,
            IsComputed = definition.CodeGenMetadata?.IsComputed ?? false,
            IsReadOnly = definition.CodeGenMetadata?.IsReadOnly ?? false,
            CodeGenSettings = definition.CodeGenMetadata,
            DatabaseSettings = definition.DatabaseMetadata,
            DisplaySettings = definition.CodeGenMetadata?.DisplaySettings,
            OriginalDefinition = definition
        };

        // Determine type
        property.TypeName = ResolveTypeName(definition);
        property.DataType = ResolveDataType(definition);

        // Parse enum values
        if (definition.Enum != null)
        {
            property.EnumValues = definition.Enum.Select((v, i) => new EnumValue
            {
                Name = v?.ToString() ?? $"Value{i}",
                Value = i
            }).ToList();
        }

        // Parse validation rules
        if (definition.CodeGenMetadata?.ValidationRules != null)
        {
            property.ValidationRules = definition.CodeGenMetadata.ValidationRules;
        }

        // Parse custom attributes
        if (definition.CodeGenMetadata?.CustomAttributes != null)
        {
            foreach (var attr in definition.CodeGenMetadata.CustomAttributes)
            {
                property.Attributes.Add(new AttributeModel
                {
                    Name = attr.Name,
                    NamedArguments = attr.Arguments ?? new Dictionary<string, object>()
                });
            }
        }

        return property;
    }

    private string ResolveTypeName(PropertyDefinition definition)
    {
        // Check for explicit CLR type
        if (!string.IsNullOrEmpty(definition.CodeGenMetadata?.ClrType))
            return definition.CodeGenMetadata.ClrType;

        // Check for reference
        if (!string.IsNullOrEmpty(definition.Ref))
        {
            // Extract entity name from $ref
            var refParts = definition.Ref.Split('/');
            return refParts.LastOrDefault() ?? "object";
        }

        // Map JSON Schema types to CLR types
        return (definition.Type?.ToLowerInvariant(), definition.Format?.ToLowerInvariant()) switch
        {
            ("string", "date-time") => "DateTime",
            ("string", "date") => "DateOnly",
            ("string", "time") => "TimeOnly",
            ("string", "uuid") => "Guid",
            ("string", "uri") => "Uri",
            ("string", "email") => "string",
            ("string", _) => "string",
            ("integer", "int64") => "long",
            ("integer", _) => "int",
            ("number", "double") => "double",
            ("number", "float") => "float",
            ("number", _) => "decimal",
            ("boolean", _) => "bool",
            ("array", _) => ResolveArrayType(definition),
            ("object", _) => "object",
            (null, _) when definition.Ref != null => definition.Ref.Split('/').LastOrDefault() ?? "object",
            _ => "object"
        };
    }

    private string ResolveArrayType(PropertyDefinition definition)
    {
        if (definition.Items == null)
            return "List<object>";

        var itemType = ResolveTypeName(definition.Items);
        return $"List<{itemType}>";
    }

    private PropertyDataType ResolveDataType(PropertyDefinition definition)
    {
        if (definition.Enum != null)
            return PropertyDataType.Enum;

        return (definition.Type?.ToLowerInvariant(), definition.Format?.ToLowerInvariant()) switch
        {
            ("string", "date-time") => PropertyDataType.DateTime,
            ("string", "date") => PropertyDataType.DateOnly,
            ("string", "time") => PropertyDataType.TimeOnly,
            ("string", "uuid") => PropertyDataType.Guid,
            ("string", _) => PropertyDataType.String,
            ("integer", "int64") => PropertyDataType.Long,
            ("integer", _) => PropertyDataType.Integer,
            ("number", "double") => PropertyDataType.Double,
            ("number", "float") => PropertyDataType.Float,
            ("number", _) => PropertyDataType.Decimal,
            ("boolean", _) => PropertyDataType.Boolean,
            ("array", _) => PropertyDataType.Array,
            ("object", _) => PropertyDataType.Object,
            _ => PropertyDataType.Object
        };
    }

    private EnumModel ParseEnum(string name, PropertyDefinition definition)
    {
        var enumModel = new EnumModel
        {
            Name = name,
            Description = definition.Description
        };

        if (definition.Enum != null)
        {
            for (int i = 0; i < definition.Enum.Count; i++)
            {
                enumModel.Values.Add(new EnumValue
                {
                    Name = definition.Enum[i]?.ToString() ?? $"Value{i}",
                    Value = i
                });
            }
        }

        return enumModel;
    }

    private void ResolveRelationships(DomainContext context)
    {
        var entityLookup = context.Entities.ToDictionary(e => e.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var entity in context.Entities)
        {
            foreach (var property in entity.Properties.ToList())
            {
                // Check if this property references another entity
                if (property.OriginalDefinition?.Ref != null)
                {
                    var refName = property.OriginalDefinition.Ref.Split('/').LastOrDefault();
                    if (refName != null && entityLookup.TryGetValue(refName, out var targetEntity))
                    {
                        var nav = new NavigationProperty
                        {
                            Name = property.Name,
                            TargetEntity = refName,
                            RelationshipType = RelationshipType.ManyToOne,
                            IsNullable = property.IsNullable
                        };

                        // Try to find foreign key property
                        var fkName = $"{property.Name}Id";
                        var fkProperty = entity.Properties.FirstOrDefault(p =>
                            p.Name.Equals(fkName, StringComparison.OrdinalIgnoreCase));
                        if (fkProperty != null)
                        {
                            nav.ForeignKeyProperty = fkProperty.Name;
                            fkProperty.IsForeignKey = true;
                        }

                        entity.NavigationProperties.Add(nav);
                    }
                }

                // Check for array of references (one-to-many)
                if (property.DataType == PropertyDataType.Array && property.OriginalDefinition?.Items?.Ref != null)
                {
                    var refName = property.OriginalDefinition.Items.Ref.Split('/').LastOrDefault();
                    if (refName != null && entityLookup.ContainsKey(refName))
                    {
                        entity.NavigationProperties.Add(new NavigationProperty
                        {
                            Name = property.Name,
                            TargetEntity = refName,
                            RelationshipType = RelationshipType.OneToMany,
                            IsPrincipal = true
                        });
                    }
                }
            }
        }
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
