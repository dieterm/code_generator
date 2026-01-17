using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.DataTypes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeGenerator.Core.Workspaces.Datasources.Yaml.Services;

/// <summary>
/// Service for reading schema and data from YAML files
/// </summary>
public class YamlSchemaReader
{
    private readonly IDeserializer _deserializer;

    public YamlSchemaReader()
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    /// <summary>
    /// Get information about a YAML file
    /// </summary>
    public Task<YamlFileInfo> GetFileInfoAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var result = new YamlFileInfo
        {
            FileName = Path.GetFileName(filePath),
            TableName = Path.GetFileNameWithoutExtension(filePath)
        };

        if (!File.Exists(filePath))
            return Task.FromResult(result);

        var yamlContent = File.ReadAllText(filePath);
        var root = _deserializer.Deserialize<object>(yamlContent);

        if (root is List<object> list)
        {
            result.IsSequence = true;
            result.ItemCount = list.Count;
            
            // Sample properties from sequence items
            result.Properties = SamplePropertiesFromSequence(list);
            result.PropertyCount = result.Properties.Count;
        }
        else if (root is Dictionary<object, object> dict)
        {
            result.IsSequence = false;
            result.ItemCount = 1;
            
            // Get properties from single mapping
            result.Properties = GetPropertiesFromMapping(dict);
            result.PropertyCount = result.Properties.Count;
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Import a YAML file as a table with its columns
    /// </summary>
    public Task<TableArtifact> ImportYamlAsync(
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

        var yamlContent = File.ReadAllText(filePath);
        var root = _deserializer.Deserialize<object>(yamlContent);

        // Extract columns by sampling all objects
        if (root is Dictionary<object, object> dict)
        {
            ExtractColumnArtifactsFromMapping(table, dict);
        }
        else if (root is List<object> list)
        {
            ExtractColumnArtifactsFromSequence(table, list);
        }

        return Task.FromResult(table);
    }

    /// <summary>
    /// Extract column artifacts from a single YAML mapping (object)
    /// </summary>
    private void ExtractColumnArtifactsFromMapping(IArtifact parent, Dictionary<object, object> mapping)
    {
        int ordinal = 1;
        foreach (var kvp in mapping)
        {
            var propName = kvp.Key?.ToString() ?? $"property{ordinal}";
            var propInfo = new YamlPropertyInfo
            {
                Name = propName,
                InferredType = InferTypeFromValue(kvp.Value),
                IsNullable = kvp.Value == null,
                IsMapping = kvp.Value is Dictionary<object, object>,
                IsSequence = kvp.Value is List<object>
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

            // Create sub-table for nested structures
            var subTable = new TableArtifact(column.Name, string.Empty);

            // Recursively add nested columns for mapping properties
            if (propInfo.IsMapping && kvp.Value is Dictionary<object, object> nestedDict)
            {
                ExtractColumnArtifactsFromMapping(subTable, nestedDict);
            }
            else if (propInfo.IsSequence && kvp.Value is List<object> nestedList)
            {
                ExtractColumnArtifactsFromSequence(subTable, nestedList);
            }

            if (subTable.Children.Any())
            {
                column.AddChild(subTable);
            }
        }
    }

    /// <summary>
    /// Extract columns from a YAML sequence by sampling all objects and creating a union of properties
    /// </summary>
    private void ExtractColumnArtifactsFromSequence(IArtifact parent, List<object> sequence, int maxSampleSize = 100)
    {
        // First, collect a union of all properties from all mappings in the sequence
        var propertyUnion = new Dictionary<string, YamlPropertyUnionInfo>();
        int sampleCount = 0;

        foreach (var item in sequence)
        {
            if (sampleCount >= maxSampleSize)
                break;

            if (item is Dictionary<object, object> dict)
            {
                foreach (var kvp in dict)
                {
                    var propName = kvp.Key?.ToString() ?? "unknown";
                    
                    if (!propertyUnion.TryGetValue(propName, out var unionInfo))
                    {
                        // First time seeing this property
                        unionInfo = new YamlPropertyUnionInfo
                        {
                            Name = propName,
                            InferredType = InferTypeFromValue(kvp.Value),
                            IsNullable = kvp.Value == null,
                            IsMapping = kvp.Value is Dictionary<object, object>,
                            IsSequence = kvp.Value is List<object>,
                            SampleValues = new List<object>()
                        };
                        propertyUnion[propName] = unionInfo;
                    }
                    else
                    {
                        // Update nullability if we find a null value
                        if (kvp.Value == null)
                        {
                            unionInfo.IsNullable = true;
                        }
                        // If the property was previously seen as non-mapping/non-sequence but now is, update
                        if (kvp.Value is Dictionary<object, object> && !unionInfo.IsMapping)
                        {
                            unionInfo.IsMapping = true;
                            unionInfo.InferredType = GenericDataTypes.Json.Id;
                        }
                        if (kvp.Value is List<object> && !unionInfo.IsSequence)
                        {
                            unionInfo.IsSequence = true;
                            unionInfo.InferredType = GenericDataTypes.Json.Id;
                        }
                    }

                    // Collect sample values for nested extraction (limit to avoid memory issues)
                    if (unionInfo.SampleValues.Count < maxSampleSize && 
                        (kvp.Value is Dictionary<object, object> || kvp.Value is List<object>))
                    {
                        unionInfo.SampleValues.Add(kvp.Value);
                    }
                }
                sampleCount++;
            }
        }

        // Mark properties as nullable if they don't appear in all sampled objects
        foreach (var unionInfo in propertyUnion.Values)
        {
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
            if (unionInfo.IsMapping && unionInfo.SampleValues.Count > 0)
            {
                ExtractNestedColumnsFromSamples(column, unionInfo.SampleValues, isSequence: false);
            }
            else if (unionInfo.IsSequence && unionInfo.SampleValues.Count > 0)
            {
                ExtractNestedColumnsFromSamples(column, unionInfo.SampleValues, isSequence: true);
            }
        }
    }

    /// <summary>
    /// Extract nested columns from multiple sample values (union of all properties)
    /// </summary>
    private void ExtractNestedColumnsFromSamples(ColumnArtifact parentColumn, List<object> samples, bool isSequence)
    {
        var nestedPropertyUnion = new Dictionary<string, YamlPropertyUnionInfo>();

        foreach (var sample in samples)
        {
            if (isSequence && sample is List<object> list)
            {
                // For sequences, scan all mappings within each sequence sample
                foreach (var item in list)
                {
                    if (item is Dictionary<object, object> dict)
                    {
                        MergeMappingPropertiesIntoUnion(nestedPropertyUnion, dict);
                    }
                }
            }
            else if (!isSequence && sample is Dictionary<object, object> dict)
            {
                // For mappings, merge properties directly
                MergeMappingPropertiesIntoUnion(nestedPropertyUnion, dict);
            }
        }

        // Create sub-table
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
            if (unionInfo.IsMapping && unionInfo.SampleValues.Count > 0)
            {
                ExtractNestedColumnsFromSamples(column, unionInfo.SampleValues, isSequence: false);
            }
            else if (unionInfo.IsSequence && unionInfo.SampleValues.Count > 0)
            {
                ExtractNestedColumnsFromSamples(column, unionInfo.SampleValues, isSequence: true);
            }
        }

        if (subTable.Children.Any())
        {
            parentColumn.AddChild(subTable);
        }
    }

    /// <summary>
    /// Merge properties from a YAML mapping into a property union dictionary
    /// </summary>
    private void MergeMappingPropertiesIntoUnion(Dictionary<string, YamlPropertyUnionInfo> union, Dictionary<object, object> mapping)
    {
        foreach (var kvp in mapping)
        {
            var propName = kvp.Key?.ToString() ?? "unknown";
            
            if (!union.TryGetValue(propName, out var unionInfo))
            {
                unionInfo = new YamlPropertyUnionInfo
                {
                    Name = propName,
                    InferredType = InferTypeFromValue(kvp.Value),
                    IsNullable = kvp.Value == null,
                    IsMapping = kvp.Value is Dictionary<object, object>,
                    IsSequence = kvp.Value is List<object>,
                    SampleValues = new List<object>()
                };
                union[propName] = unionInfo;
            }
            else
            {
                if (kvp.Value == null)
                {
                    unionInfo.IsNullable = true;
                }
                if (kvp.Value is Dictionary<object, object>)
                {
                    unionInfo.IsMapping = true;
                }
                if (kvp.Value is List<object>)
                {
                    unionInfo.IsSequence = true;
                }
            }

            // Collect samples for further nesting (limit to 10 per property to avoid memory issues)
            if (unionInfo.SampleValues.Count < 10 &&
                (kvp.Value is Dictionary<object, object> || kvp.Value is List<object>))
            {
                unionInfo.SampleValues.Add(kvp.Value);
            }
        }
    }

    /// <summary>
    /// Read all data from a YAML file as dynamic objects
    /// </summary>
    public Task<List<Dictionary<string, object?>>> ReadYamlDataAsync(
        string filePath,
        int? maxRows = null,
        CancellationToken cancellationToken = default)
    {
        var results = new List<Dictionary<string, object?>>();

        if (!File.Exists(filePath))
            return Task.FromResult(results);

        var yamlContent = File.ReadAllText(filePath);
        var root = _deserializer.Deserialize<object>(yamlContent);

        if (root is List<object> list)
        {
            int count = 0;
            foreach (var item in list)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (maxRows.HasValue && count >= maxRows.Value)
                    break;

                if (item is Dictionary<object, object> dict)
                {
                    results.Add(ConvertYamlMappingToDictionary(dict));
                    count++;
                }
            }
        }
        else if (root is Dictionary<object, object> dict)
        {
            results.Add(ConvertYamlMappingToDictionary(dict));
        }

        return Task.FromResult(results);
    }

    /// <summary>
    /// Sample properties from a sequence of YAML mappings
    /// </summary>
    private List<YamlPropertyInfo> SamplePropertiesFromSequence(List<object> sequence)
    {
        var propertyMap = new Dictionary<string, YamlPropertyInfo>();
        int sampleSize = Math.Min(100, sequence.Count);
        int count = 0;

        foreach (var item in sequence)
        {
            if (count >= sampleSize)
                break;

            if (item is Dictionary<object, object> dict)
            {
                foreach (var kvp in dict)
                {
                    var propName = kvp.Key?.ToString() ?? "unknown";
                    
                    if (!propertyMap.ContainsKey(propName))
                    {
                        propertyMap[propName] = new YamlPropertyInfo
                        {
                            Name = propName,
                            InferredType = InferTypeFromValue(kvp.Value),
                            IsNullable = kvp.Value == null,
                            IsMapping = kvp.Value is Dictionary<object, object>,
                            IsSequence = kvp.Value is List<object>
                        };
                    }
                    else
                    {
                        // Update nullability if we find a null value
                        if (kvp.Value == null)
                        {
                            propertyMap[propName].IsNullable = true;
                        }
                    }
                }
            }
            count++;
        }

        return propertyMap.Values.ToList();
    }

    /// <summary>
    /// Get properties from a single YAML mapping
    /// </summary>
    private List<YamlPropertyInfo> GetPropertiesFromMapping(Dictionary<object, object> mapping)
    {
        var properties = new List<YamlPropertyInfo>();

        foreach (var kvp in mapping)
        {
            properties.Add(new YamlPropertyInfo
            {
                Name = kvp.Key?.ToString() ?? "unknown",
                InferredType = InferTypeFromValue(kvp.Value),
                IsNullable = kvp.Value == null,
                IsMapping = kvp.Value is Dictionary<object, object>,
                IsSequence = kvp.Value is List<object>
            });
        }

        return properties;
    }

    /// <summary>
    /// Infer the data type from a YAML value
    /// </summary>
    private string InferTypeFromValue(object? value)
    {
        return value switch
        {
            null => GenericDataTypes.VarChar.Id,
            bool => GenericDataTypes.Boolean.Id,
            int or long => GenericDataTypes.BigInt.Id,
            float or double or decimal => GenericDataTypes.Decimal.Id,
            DateTime => GenericDataTypes.DateTime.Id,
            Dictionary<object, object> => GenericDataTypes.Json.Id,
            List<object> => GenericDataTypes.Json.Id,
            string s => TryInferStringType(s),
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
    /// Convert a YAML mapping to a dictionary, preserving nested structures
    /// </summary>
    private Dictionary<string, object?> ConvertYamlMappingToDictionary(Dictionary<object, object> mapping)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var kvp in mapping)
        {
            var key = kvp.Key?.ToString() ?? "unknown";
            dict[key] = ConvertYamlValue(kvp.Value);
        }

        return dict;
    }

    /// <summary>
    /// Convert a YAML value to a .NET object, preserving structure
    /// </summary>
    private object? ConvertYamlValue(object? value)
    {
        return value switch
        {
            null => null,
            Dictionary<object, object> dict => ConvertYamlMappingToDictionary(dict),
            List<object> list => list.Select(ConvertYamlValue).ToList(),
            _ => value
        };
    }
}
