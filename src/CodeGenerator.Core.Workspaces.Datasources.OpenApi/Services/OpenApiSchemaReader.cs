using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.DataTypes;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace CodeGenerator.Core.Workspaces.Datasources.OpenApi.Services;

/// <summary>
/// Service for reading schema information from OpenAPI/Swagger specification files
/// </summary>
public class OpenApiSchemaReader
{
    private const int MaxNestedDepth = 5;

    /// <summary>
    /// Get information about an OpenAPI specification file
    /// </summary>
    public async Task<OpenApiFileInfo> GetFileInfoAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var result = new OpenApiFileInfo
        {
            FileName = Path.GetFileName(filePath)
        };

        if (!File.Exists(filePath))
            return result;

        try
        {
            using var stream = File.OpenRead(filePath);
            var document = await new OpenApiStreamReader().ReadAsync(stream, cancellationToken);
            var openApiDoc = document.OpenApiDocument;

            if (openApiDoc == null)
                return result;

            result.Title = openApiDoc.Info?.Title;
            result.Version = openApiDoc.Info?.Version;
            result.Description = openApiDoc.Info?.Description;

            // Get schemas from components
            if (openApiDoc.Components?.Schemas != null)
            {
                foreach (var schema in openApiDoc.Components.Schemas)
                {
                    var schemaInfo = CreateSchemaInfo(schema.Key, schema.Value);
                    result.Schemas.Add(schemaInfo);
                }
            }

            result.SchemaCount = result.Schemas.Count;
        }
        catch (Exception ex)
        {
            result.Title = $"Error: {ex.Message}";
        }

        return result;
    }

    /// <summary>
    /// Import schemas from an OpenAPI file as TableArtifacts
    /// </summary>
    public async Task<List<TableArtifact>> ImportSchemasAsync(
        string filePath,
        string datasourceName,
        IEnumerable<string>? schemaFilter = null,
        CancellationToken cancellationToken = default)
    {
        var tables = new List<TableArtifact>();

        if (!File.Exists(filePath))
            return tables;

        try
        {
            using var stream = File.OpenRead(filePath);
            var document = await new OpenApiStreamReader().ReadAsync(stream, cancellationToken);
            var openApiDoc = document.OpenApiDocument;

            if (openApiDoc?.Components?.Schemas == null)
                return tables;

            var schemas = openApiDoc.Components.Schemas.AsEnumerable();

            // Apply filter if provided
            if (schemaFilter != null && schemaFilter.Any())
            {
                var filterSet = schemaFilter.ToHashSet(StringComparer.OrdinalIgnoreCase);
                schemas = schemas.Where(s => filterSet.Contains(s.Key));
            }

            foreach (var schema in schemas)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var table = CreateTableFromSchema(schema.Key, schema.Value, datasourceName, openApiDoc.Components.Schemas, 0);
                tables.Add(table);
            }
        }
        catch (Exception)
        {
            // Return empty list on error
        }

        return tables;
    }

    /// <summary>
    /// Import a single schema as a TableArtifact
    /// </summary>
    public async Task<TableArtifact> ImportSchemaAsync(
        string filePath,
        string schemaName,
        string datasourceName,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("OpenAPI file not found", filePath);

        using var stream = File.OpenRead(filePath);
        var document = await new OpenApiStreamReader().ReadAsync(stream, cancellationToken);
        var openApiDoc = document.OpenApiDocument;

        if (openApiDoc?.Components?.Schemas == null)
            throw new InvalidOperationException("No schemas found in OpenAPI specification");

        if (!openApiDoc.Components.Schemas.TryGetValue(schemaName, out var schema))
            throw new InvalidOperationException($"Schema '{schemaName}' not found in OpenAPI specification");

        return CreateTableFromSchema(schemaName, schema, datasourceName, openApiDoc.Components.Schemas, 0);
    }

    private OpenApiSchemaInfo CreateSchemaInfo(string name, OpenApiSchema schema)
    {
        var info = new OpenApiSchemaInfo
        {
            Name = name,
            Description = schema.Description,
            SchemaType = schema.Type ?? "object",
            IsEnum = schema.Enum?.Count > 0
        };

        if (info.IsEnum)
        {
            info.EnumValues = schema.Enum?
                .Select(e => e.ToString())
                .ToList();
        }

        // Get properties
        if (schema.Properties != null)
        {
            var requiredProps = schema.Required ?? new HashSet<string>();

            foreach (var prop in schema.Properties)
            {
                var propInfo = CreatePropertyInfo(prop.Key, prop.Value, requiredProps.Contains(prop.Key));
                info.Properties.Add(propInfo);
            }
        }

        info.PropertyCount = info.Properties.Count;

        return info;
    }

    private OpenApiPropertyInfo CreatePropertyInfo(string name, OpenApiSchema propSchema, bool isRequired)
    {
        var info = new OpenApiPropertyInfo
        {
            Name = name,
            Description = propSchema.Description,
            IsRequired = isRequired,
            IsNullable = propSchema.Nullable || !isRequired
        };

        // Check if it's a reference
        if (propSchema.Reference != null)
        {
            info.IsReference = true;
            info.ReferencedSchemaName = propSchema.Reference.Id;
            info.DataType = "object";
            info.InferredGenericType = GenericDataTypes.Json.Id;
        }
        // Check if it's an array
        else if (propSchema.Type == "array" && propSchema.Items != null)
        {
            info.IsArray = true;

            if (propSchema.Items.Reference != null)
            {
                info.IsReference = true;
                info.ReferencedSchemaName = propSchema.Items.Reference.Id;
                info.DataType = "object";
            }
            else
            {
                info.DataType = propSchema.Items.Type ?? "object";
                info.Format = propSchema.Items.Format;
            }

            info.InferredGenericType = GenericDataTypes.Json.Id;
        }
        else
        {
            info.DataType = propSchema.Type ?? "string";
            info.Format = propSchema.Format;
            info.InferredGenericType = InferGenericDataType(propSchema.Type, propSchema.Format);
        }

        return info;
    }

    private TableArtifact CreateTableFromSchema(
        string schemaName,
        OpenApiSchema schema,
        string datasourceName,
        IDictionary<string, OpenApiSchema> allSchemas,
        int depth)
    {
        var table = new TableArtifact(schemaName, string.Empty);

        // Add decorator to mark as existing
        table.AddDecorator(new ExistingTableDecorator
        {
            OriginalTableName = schemaName,
            OriginalSchema = string.Empty,
            ImportedAt = DateTime.UtcNow,
            SourceDatasourceName = datasourceName
        });

        // For enums, add the enum values as columns
        if (schema.Enum?.Count > 0)
        {
            int ordinal = 1;
            foreach (var enumValue in schema.Enum)
            {
                var valueName = enumValue.ToString();
                var column = new ColumnArtifact(valueName, GenericDataTypes.VarChar.Id, false)
                {
                    OrdinalPosition = ordinal++
                };
                column.AddDecorator(new ExistingColumnDecorator
                {
                    OriginalName = valueName,
                    OriginalDataType = GenericDataTypes.VarChar.Id,
                    OriginalOrdinalPosition = column.OrdinalPosition,
                    OriginalIsNullable = false
                });
                table.AddChild(column);
            }
            return table;
        }

        // Get properties
        if (schema.Properties != null)
        {
            var requiredProps = schema.Required ?? new HashSet<string>();
            int ordinal = 1;

            foreach (var prop in schema.Properties)
            {
                var isRequired = requiredProps.Contains(prop.Key);
                var propSchema = prop.Value;
                var isNullable = propSchema.Nullable || !isRequired;

                string dataType;
                bool isReference = false;
                string? referencedSchemaName = null;
                bool isArray = false;
                OpenApiSchema? nestedSchema = null;

                // Check if it's a reference
                if (propSchema.Reference != null)
                {
                    isReference = true;
                    referencedSchemaName = propSchema.Reference.Id;
                    dataType = GenericDataTypes.Json.Id;

                    if (allSchemas.TryGetValue(referencedSchemaName, out var refSchema))
                    {
                        nestedSchema = refSchema;
                    }
                }
                // Check if it's an array
                else if (propSchema.Type == "array" && propSchema.Items != null)
                {
                    isArray = true;
                    dataType = GenericDataTypes.Json.Id;

                    if (propSchema.Items.Reference != null)
                    {
                        isReference = true;
                        referencedSchemaName = propSchema.Items.Reference.Id;

                        if (allSchemas.TryGetValue(referencedSchemaName, out var refSchema))
                        {
                            nestedSchema = refSchema;
                        }
                    }
                    else if (propSchema.Items.Properties?.Count > 0)
                    {
                        nestedSchema = propSchema.Items;
                    }
                }
                else
                {
                    dataType = InferGenericDataType(propSchema.Type, propSchema.Format);
                }

                var column = new ColumnArtifact(prop.Key, dataType, isNullable)
                {
                    OrdinalPosition = ordinal++
                };

                column.AddDecorator(new ExistingColumnDecorator
                {
                    OriginalName = prop.Key,
                    OriginalDataType = dataType,
                    OriginalOrdinalPosition = column.OrdinalPosition,
                    OriginalIsNullable = isNullable
                });

                table.AddChild(column);

                // Add nested table for referenced schemas (recursive)
                if (depth < MaxNestedDepth && nestedSchema != null)
                {
                    var nestedTableName = referencedSchemaName ?? prop.Key;
                    var nestedTable = CreateTableFromSchema(
                        nestedTableName,
                        nestedSchema,
                        datasourceName,
                        allSchemas,
                        depth + 1);
                    column.AddChild(nestedTable);
                }
            }
        }

        return table;
    }

    private string InferGenericDataType(string? openApiType, string? format)
    {
        return (openApiType?.ToLowerInvariant(), format?.ToLowerInvariant()) switch
        {
            ("string", "date") => GenericDataTypes.Date.Id,
            ("string", "date-time") => GenericDataTypes.DateTime.Id,
            ("string", "uuid") => GenericDataTypes.Guid.Id,
            ("string", "guid") => GenericDataTypes.Guid.Id,
            ("string", "binary") => GenericDataTypes.Binary.Id,
            ("string", "byte") => GenericDataTypes.Binary.Id,
            ("string", _) => GenericDataTypes.VarChar.Id,
            ("integer", "int32") => GenericDataTypes.Int.Id,
            ("integer", "int64") => GenericDataTypes.BigInt.Id,
            ("integer", _) => GenericDataTypes.Int.Id,
            ("number", "float") => GenericDataTypes.Float.Id,
            ("number", "double") => GenericDataTypes.Double.Id,
            ("number", _) => GenericDataTypes.Decimal.Id,
            ("boolean", _) => GenericDataTypes.Boolean.Id,
            ("object", _) => GenericDataTypes.Json.Id,
            ("array", _) => GenericDataTypes.Json.Id,
            _ => GenericDataTypes.VarChar.Id
        };
    }
}
