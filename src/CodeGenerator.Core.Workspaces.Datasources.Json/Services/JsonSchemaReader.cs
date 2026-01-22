using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.DataTypes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        
        // Extract columns by sampling all objects
        if (root.ValueKind == JsonValueKind.Object)
        {
            ExtractColumnArtifactsFromObject(table, root);
        }
        else if (root.ValueKind == JsonValueKind.Array)
        {
            ExtractColumnArtifactsFromArray(table, root);
        }
        
        return Task.FromResult(table);
    }

    /// <summary>
    /// Extract column artifacts from a single JSON object
    /// </summary>
    private void ExtractColumnArtifactsFromObject(IArtifact parent, JsonElement objectElement)
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

            // create sub-table
            var subTable = new TableArtifact(column.Name, string.Empty);

            // Recursively add nested columns for object properties
            if (propInfo.IsObject)
            {
                ExtractColumnArtifactsFromObject(subTable, prop.Value);
            }
            else if (propInfo.IsArray)
            {
                // If the array contains objects, extract columns from all objects (union)
                ExtractColumnArtifactsFromArray(subTable, prop.Value);
            }
            if(subTable.Children.Any())
            {
                column.AddChild(subTable);
            }
        }
    }

    /// <summary>
    /// Extract columns from a JSON array by sampling all objects and creating a union of properties
    /// </summary>
    private void ExtractColumnArtifactsFromArray(IArtifact parent, JsonElement arrayElement, int maxSampleSize = 100)
    {
        if (arrayElement.ValueKind != JsonValueKind.Array)
            return;

        // First, collect a union of all properties from all objects in the array
        var propertyUnion = new Dictionary<string, JsonPropertyUnionInfo>();
        int sampleCount = 0;

        foreach (var item in arrayElement.EnumerateArray())
        {
            if (sampleCount >= maxSampleSize)
                break;

            if (item.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in item.EnumerateObject())
                {
                    if (!propertyUnion.TryGetValue(prop.Name, out var unionInfo))
                    {
                        // First time seeing this property
                        unionInfo = new JsonPropertyUnionInfo
                        {
                            Name = prop.Name,
                            InferredType = InferTypeFromValue(prop.Value),
                            IsNullable = prop.Value.ValueKind == JsonValueKind.Null,
                            IsObject = prop.Value.ValueKind == JsonValueKind.Object,
                            IsArray = prop.Value.ValueKind == JsonValueKind.Array,
                            SampleValues = new List<JsonElement>()
                        };
                        propertyUnion[prop.Name] = unionInfo;
                    }
                    else
                    {
                        // Update nullability if we find a null value
                        if (prop.Value.ValueKind == JsonValueKind.Null)
                        {
                            unionInfo.IsNullable = true;
                        }
                        // If the property was previously seen as non-object/non-array but now is, update
                        if (prop.Value.ValueKind == JsonValueKind.Object && !unionInfo.IsObject)
                        {
                            unionInfo.IsObject = true;
                            unionInfo.InferredType = GenericDataTypes.Json.Id;
                        }
                        if (prop.Value.ValueKind == JsonValueKind.Array && !unionInfo.IsArray)
                        {
                            unionInfo.IsArray = true;
                            unionInfo.InferredType = GenericDataTypes.Json.Id;
                        }
                    }

                    // Collect sample values for nested extraction (limit to avoid memory issues)
                    if (unionInfo.SampleValues.Count < maxSampleSize && 
                        (prop.Value.ValueKind == JsonValueKind.Object || prop.Value.ValueKind == JsonValueKind.Array))
                    {
                        unionInfo.SampleValues.Add(prop.Value.Clone());
                    }
                }
                sampleCount++;
            }
        }

        // Mark properties as nullable if they don't appear in all sampled objects
        // (they are optional)
        foreach (var unionInfo in propertyUnion.Values)
        {
            // If a property doesn't appear in all objects, it's nullable/optional
            // We can't easily count this without another pass, so we'll be conservative
            // and mark all properties from arrays as nullable
            unionInfo.IsNullable = true;
        }
        

        // Now create column artifacts from the union
        int ordinal = 1;
        foreach (var unionInfo in propertyUnion.Values)
        {
            var column = new ColumnArtifact(unionInfo.Name, unionInfo.InferredType, unionInfo.IsNullable)
            {
                OrdinalPosition = ordinal++
            };

            // Add decorator to mark as existing
            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = unionInfo.Name,
                OriginalDataType = unionInfo.InferredType,
                OriginalOrdinalPosition = column.OrdinalPosition,
                OriginalIsNullable = unionInfo.IsNullable
            });

            parent.AddChild(column);

            // Recursively extract nested columns
            if (unionInfo.IsObject && unionInfo.SampleValues.Count > 0)
            {
                // Create a merged view of all object samples
                ExtractNestedColumnsFromSamples(column, unionInfo.SampleValues, isArray: false);
            }
            else if (unionInfo.IsArray && unionInfo.SampleValues.Count > 0)
            {
                // Extract from all array samples
                ExtractNestedColumnsFromSamples(column, unionInfo.SampleValues, isArray: true);
            }
        }
        //if(subTable.Children.Any())
        //{
        //    parent.AddChild(subTable);
        //}   
    }

    /// <summary>
    /// Extract nested columns from multiple sample values (union of all properties)
    /// </summary>
    private void ExtractNestedColumnsFromSamples(ColumnArtifact parentColumn, List<JsonElement> samples, bool isArray)
    {
        var nestedPropertyUnion = new Dictionary<string, JsonPropertyUnionInfo>();

        foreach (var sample in samples)
        {
            if (isArray && sample.ValueKind == JsonValueKind.Array)
            {
                // For arrays, scan all objects within each array sample
                foreach (var item in sample.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        MergeObjectPropertiesIntoUnion(nestedPropertyUnion, item);
                    }
                }
            }
            else if (!isArray && sample.ValueKind == JsonValueKind.Object)
            {
                // For objects, merge properties directly
                MergeObjectPropertiesIntoUnion(nestedPropertyUnion, sample);
            }
        }
        // create sub-table
        var subTable = new TableArtifact(parentColumn.Name, string.Empty);
        
        int ordinal = 1;
        foreach (var unionInfo in nestedPropertyUnion.Values)
        {
            var column = new ColumnArtifact(unionInfo.Name, unionInfo.InferredType, true) // Always nullable for nested
            {
                OrdinalPosition = ordinal++
            };

            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = unionInfo.Name,
                OriginalDataType = unionInfo.InferredType,
                OriginalOrdinalPosition = column.OrdinalPosition,
                OriginalIsNullable = true
            });

            subTable.AddChild(column);

            // Continue recursion for nested structures
            if (unionInfo.IsObject && unionInfo.SampleValues.Count > 0)
            {
                ExtractNestedColumnsFromSamples(column, unionInfo.SampleValues, isArray: false);
            }
            else if (unionInfo.IsArray && unionInfo.SampleValues.Count > 0)
            {
                ExtractNestedColumnsFromSamples(column, unionInfo.SampleValues, isArray: true);
            }
        }
        if(subTable.Children.Any())
        {
            parentColumn.AddChild(subTable);
        }
    }

    /// <summary>
    /// Merge properties from a JSON object into a property union dictionary
    /// </summary>
    private void MergeObjectPropertiesIntoUnion(Dictionary<string, JsonPropertyUnionInfo> union, JsonElement objectElement)
    {
        foreach (var prop in objectElement.EnumerateObject())
        {
            if (!union.TryGetValue(prop.Name, out var unionInfo))
            {
                unionInfo = new JsonPropertyUnionInfo
                {
                    Name = prop.Name,
                    InferredType = InferTypeFromValue(prop.Value),
                    IsNullable = prop.Value.ValueKind == JsonValueKind.Null,
                    IsObject = prop.Value.ValueKind == JsonValueKind.Object,
                    IsArray = prop.Value.ValueKind == JsonValueKind.Array,
                    SampleValues = new List<JsonElement>()
                };
                union[prop.Name] = unionInfo;
            }
            else
            {
                if (prop.Value.ValueKind == JsonValueKind.Null)
                {
                    unionInfo.IsNullable = true;
                }
                if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    unionInfo.IsObject = true;
                }
                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    unionInfo.IsArray = true;
                }
            }

            // Collect samples for further nesting (limit to 10 per property to avoid memory issues)
            if (unionInfo.SampleValues.Count < 10 &&
                (prop.Value.ValueKind == JsonValueKind.Object || prop.Value.ValueKind == JsonValueKind.Array))
            {
                unionInfo.SampleValues.Add(prop.Value.Clone());
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
