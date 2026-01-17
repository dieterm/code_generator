using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.DataTypes;
using System.Diagnostics;
using System.Text.Json;

namespace CodeGenerator.Core.Workspaces.Datasources.Json.Services;

/// <summary>
/// Service for reading schema and data from JSON files
/// </summary>
public class JsonSchemaReader
{
    /// <summary>
    /// Get information about a JSON file
    /// </summary>
    public Task<JsonFileInfo> GetFileInfoAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var result = new JsonFileInfo
        {
            FileName = Path.GetFileName(filePath),
            TableName = Path.GetFileNameWithoutExtension(filePath)
        };

        if (!File.Exists(filePath))
            return Task.FromResult(result);

        var jsonContent = File.ReadAllText(filePath);
        using var document = JsonDocument.Parse(jsonContent);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            result.IsArray = true;
            result.ItemCount = root.GetArrayLength();
            
            // Sample properties from array items
            result.Properties = SamplePropertiesFromArray(root);
            result.PropertyCount = result.Properties.Count;
        }
        else if (root.ValueKind == JsonValueKind.Object)
        {
            result.IsArray = false;
            result.ItemCount = 1;
            
            // Get properties from single object
            result.Properties = GetPropertiesFromObject(root);
            result.PropertyCount = result.Properties.Count;
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Import a JSON file as a table with its columns
    /// </summary>
    public Task<TableArtifact> ImportJsonAsync(
        string filePath,
        string datasourceName,
        CancellationToken cancellationToken = default)
    {
        var tableName = Path.GetFileNameWithoutExtension(filePath);
        var table = new TableArtifact(tableName, string.Empty);

        // Add decorator to mark as existing
        table.AddDecorator(new ExistingTableDecorator
        {
            OriginalTableName = tableName,
            OriginalSchema = string.Empty,
            ImportedAt = DateTime.UtcNow,
            SourceDatasourceName = datasourceName
        });

        if (!File.Exists(filePath))
            return Task.FromResult(table);

        var jsonContent = File.ReadAllText(filePath);
        using var document = JsonDocument.Parse(jsonContent);
        var root = document.RootElement;
        
        // Get a representative object to extract columns from
        JsonElement? representativeObject = GetRepresentativeObject(root);
        if (representativeObject.HasValue)
        {
            ExtractColumnArtifacts(table, representativeObject.Value);
        }
        
        return Task.FromResult(table);
    }

    /// <summary>
    /// Get a representative JSON object from root (either the root itself or first array item)
    /// </summary>
    private JsonElement? GetRepresentativeObject(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Object)
        {
            return root;
        }
        else if (root.ValueKind == JsonValueKind.Array)
        {
            // Return the first object in the array
            foreach (var item in root.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object)
                {
                    return item;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Recursively extract column artifacts from a JSON object
    /// </summary>
    private void ExtractColumnArtifacts(IArtifact parent, JsonElement objectElement)
    {
        if (objectElement.ValueKind != JsonValueKind.Object)
            return;

        int ordinal = 1;
        foreach (var prop in objectElement.EnumerateObject())
        {
            var propInfo = new JsonPropertyInfo
            {
                Name = prop.Name,
                InferredType = InferTypeFromValue(prop.Value),
                IsNullable = prop.Value.ValueKind == JsonValueKind.Null,
                IsObject = prop.Value.ValueKind == JsonValueKind.Object,
                IsArray = prop.Value.ValueKind == JsonValueKind.Array
            };

            var column = new ColumnArtifact(propInfo.Name, propInfo.InferredType, propInfo.IsNullable)
            {
                OrdinalPosition = ordinal++
            };

            // Add decorator to mark as existing
            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = propInfo.Name,
                OriginalDataType = propInfo.InferredType,
                OriginalOrdinalPosition = column.OrdinalPosition,
                OriginalIsNullable = propInfo.IsNullable
            });

            parent.AddChild(column);

            // Recursively add nested columns for object properties
            if (propInfo.IsObject)
            {
                ExtractColumnArtifacts(column, prop.Value);
            }
            else if (propInfo.IsArray)
            {
                // If the array contains objects, extract columns from the first object
                ExtractColumnsFromArray(column, prop.Value);
            }
        }
    }

    /// <summary>
    /// Extract columns from a JSON array (samples the first object in the array)
    /// </summary>
    private void ExtractColumnsFromArray(ColumnArtifact parentColumn, JsonElement arrayElement)
    {
        if (arrayElement.ValueKind != JsonValueKind.Array)
            return;

        // Find the first object in the array to use as a template
        foreach (var item in arrayElement.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Object)
            {
                // Recursively extract columns from this object
                ExtractColumnArtifacts(parentColumn, item);
                break; // Only sample the first object
            }
        }
    }

    /// <summary>
    /// Read all data from a JSON file as dynamic objects
    /// </summary>
    public Task<List<Dictionary<string, object?>>> ReadJsonDataAsync(
        string filePath,
        int? maxRows = null,
        CancellationToken cancellationToken = default)
    {
        var results = new List<Dictionary<string, object?>>();

        if (!File.Exists(filePath))
            return Task.FromResult(results);

        var jsonContent = File.ReadAllText(filePath);
        using var document = JsonDocument.Parse(jsonContent);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            int count = 0;
            foreach (var item in root.EnumerateArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (maxRows.HasValue && count >= maxRows.Value)
                    break;

                if (item.ValueKind == JsonValueKind.Object)
                {
                    results.Add(ConvertJsonObjectToDictionary(item));
                    count++;
                }
            }
        }
        else if (root.ValueKind == JsonValueKind.Object)
        {
            results.Add(ConvertJsonObjectToDictionary(root));
        }

        return Task.FromResult(results);
    }

    /// <summary>
    /// Sample properties from an array of JSON objects
    /// </summary>
    private List<JsonPropertyInfo> SamplePropertiesFromArray(JsonElement arrayElement)
    {
        var propertyMap = new Dictionary<string, JsonPropertyInfo>();
        int sampleSize = Math.Min(100, arrayElement.GetArrayLength());
        int count = 0;

        foreach (var item in arrayElement.EnumerateArray())
        {
            if (count >= sampleSize)
                break;

            if (item.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in item.EnumerateObject())
                {
                    if (!propertyMap.ContainsKey(prop.Name))
                    {
                        propertyMap[prop.Name] = new JsonPropertyInfo
                        {
                            Name = prop.Name,
                            InferredType = InferTypeFromValue(prop.Value),
                            IsNullable = prop.Value.ValueKind == JsonValueKind.Null,
                            IsObject = prop.Value.ValueKind == JsonValueKind.Object,
                            IsArray = prop.Value.ValueKind == JsonValueKind.Array
                        };
                    }
                    else
                    {
                        // Update nullability if we find a null value
                        if (prop.Value.ValueKind == JsonValueKind.Null)
                        {
                            propertyMap[prop.Name].IsNullable = true;
                        }
                    }
                }
            }
            count++;
        }

        return propertyMap.Values.ToList();
    }

    /// <summary>
    /// Get properties from a single JSON object
    /// </summary>
    private List<JsonPropertyInfo> GetPropertiesFromObject(JsonElement objectElement)
    {
        var properties = new List<JsonPropertyInfo>();

        foreach (var prop in objectElement.EnumerateObject())
        {
            properties.Add(new JsonPropertyInfo
            {
                Name = prop.Name,
                InferredType = InferTypeFromValue(prop.Value),
                IsNullable = prop.Value.ValueKind == JsonValueKind.Null,
                IsObject = prop.Value.ValueKind == JsonValueKind.Object,
                IsArray = prop.Value.ValueKind == JsonValueKind.Array
            });
        }

        return properties;
    }

    /// <summary>
    /// Infer the data type from a JSON value
    /// </summary>
    private string InferTypeFromValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => TryInferStringType(value.GetString()),
            JsonValueKind.Number => InferNumberType(value),
            JsonValueKind.True or JsonValueKind.False => GenericDataTypes.Boolean.Id,
            JsonValueKind.Null => GenericDataTypes.VarChar.Id,
            JsonValueKind.Object => GenericDataTypes.Json.Id,
            JsonValueKind.Array => GenericDataTypes.Json.Id,
            _ => GenericDataTypes.VarChar.Id
        };
    }

    /// <summary>
    /// Try to infer a more specific type from a string value
    /// </summary>
    private string TryInferStringType(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return GenericDataTypes.VarChar.Id;

        // Try date/time
        if (DateTime.TryParse(value, out _))
            return GenericDataTypes.DateTime.Id;

        // Try GUID
        if (Guid.TryParse(value, out _))
            return GenericDataTypes.Guid.Id;

        return GenericDataTypes.VarChar.Id;
    }

    /// <summary>
    /// Infer whether a number is integer or decimal
    /// </summary>
    private string InferNumberType(JsonElement value)
    {
        if (value.TryGetInt64(out _))
            return GenericDataTypes.BigInt.Id;
        
        return GenericDataTypes.Decimal.Id;
    }

    /// <summary>
    /// Convert a JSON object to a dictionary, preserving nested structures
    /// </summary>
    private Dictionary<string, object?> ConvertJsonObjectToDictionary(JsonElement element)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var prop in element.EnumerateObject())
        {
            dict[prop.Name] = ConvertJsonValue(prop.Value);
        }

        return dict;
    }

    /// <summary>
    /// Convert a JSON value to a .NET object, preserving structure
    /// </summary>
    private object? ConvertJsonValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number => value.TryGetInt64(out var l) ? l : value.GetDouble(),
            JsonValueKind.Object => ConvertJsonObjectToDictionary(value),
            JsonValueKind.Array => ConvertJsonArrayToList(value),
            _ => value.GetRawText()
        };
    }

    /// <summary>
    /// Convert a JSON array to a list
    /// </summary>
    private List<object?> ConvertJsonArrayToList(JsonElement arrayElement)
    {
        var list = new List<object?>();

        foreach (var item in arrayElement.EnumerateArray())
        {
            list.Add(ConvertJsonValue(item));
        }

        return list;
    }
}
