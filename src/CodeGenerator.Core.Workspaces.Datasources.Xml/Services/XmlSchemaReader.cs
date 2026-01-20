using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.DataTypes;
using System.Xml.Linq;

namespace CodeGenerator.Core.Workspaces.Datasources.Xml.Services;

/// <summary>
/// Service for reading schema and data from XML files
/// </summary>
public class XmlSchemaReader
{
    /// <summary>
    /// Get information about an XML file
    /// </summary>
    public Task<XmlFileInfo> GetFileInfoAsync(string filePath, string? rootElementPath = null, CancellationToken cancellationToken = default)
    {
        var result = new XmlFileInfo
        {
            FileName = Path.GetFileName(filePath),
            TableName = Path.GetFileNameWithoutExtension(filePath)
        };

        if (!File.Exists(filePath))
            return Task.FromResult(result);

        var doc = XDocument.Load(filePath);
        var root = doc.Root;

        if (root == null)
            return Task.FromResult(result);

        result.RootElementName = root.Name.LocalName;

        // Determine if we have repeating elements
        var childElements = root.Elements().ToList();
        var elementGroups = childElements.GroupBy(e => e.Name.LocalName).ToList();

        if (elementGroups.Count == 1 && childElements.Count > 1)
        {
            // Single type of repeating child element (like an array)
            result.HasRepeatingElements = true;
            result.RepeatingElementName = elementGroups[0].Key;
            result.ItemCount = childElements.Count;
            result.TableName = result.RepeatingElementName;

            // Sample properties from repeating elements
            result.Properties = SamplePropertiesFromElements(childElements);
            result.PropertyCount = result.Properties.Count;
        }
        else if (childElements.Count > 0)
        {
            // Mixed or single element - treat root as the data container
            result.HasRepeatingElements = false;
            result.ItemCount = 1;

            // Get properties from root element
            result.Properties = GetPropertiesFromElement(root);
            result.PropertyCount = result.Properties.Count;
        }
        else
        {
            // Root only has attributes or text content
            result.HasRepeatingElements = false;
            result.ItemCount = 1;
            result.Properties = GetPropertiesFromElement(root);
            result.PropertyCount = result.Properties.Count;
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Import an XML file as a table with its columns
    /// </summary>
    public Task<TableArtifact> ImportXmlAsync(
        string filePath,
        string datasourceName,
        string? rootElementPath = null,
        CancellationToken cancellationToken = default)
    {
        var tableName = Path.GetFileNameWithoutExtension(filePath);

        if (!File.Exists(filePath))
        {
            var emptyTable = new TableArtifact(tableName, string.Empty);
            return Task.FromResult(emptyTable);
        }

        var doc = XDocument.Load(filePath);
        var root = doc.Root;

        if (root == null)
        {
            var emptyTable = new TableArtifact(tableName, string.Empty);
            return Task.FromResult(emptyTable);
        }

        // Determine table name from repeating element or root
        var childElements = root.Elements().ToList();
        var elementGroups = childElements.GroupBy(e => e.Name.LocalName).ToList();

        if (elementGroups.Count == 1 && childElements.Count > 1)
        {
            tableName = elementGroups[0].Key;
        }

        var table = new TableArtifact(tableName, string.Empty);

        // Add decorator to mark as existing
        table.AddDecorator(new ExistingTableDecorator
        {
            OriginalTableName = tableName,
            OriginalSchema = string.Empty,
            ImportedAt = DateTime.UtcNow,
            SourceDatasourceName = datasourceName
        });

        // Extract columns
        if (elementGroups.Count == 1 && childElements.Count > 1)
        {
            // Repeating elements - extract from all of them
            ExtractColumnArtifactsFromElements(table, childElements);
        }
        else if (root != null)
        {
            // Single root element
            ExtractColumnArtifactsFromElement(table, root);
        }

        return Task.FromResult(table);
    }

    /// <summary>
    /// Read all data from an XML file as dynamic objects
    /// </summary>
    public Task<List<Dictionary<string, object?>>> ReadXmlDataAsync(
        string filePath,
        string? rootElementPath = null,
        int? maxRows = null,
        CancellationToken cancellationToken = default)
    {
        var results = new List<Dictionary<string, object?>>();

        if (!File.Exists(filePath))
            return Task.FromResult(results);

        var doc = XDocument.Load(filePath);
        var root = doc.Root;

        if (root == null)
            return Task.FromResult(results);

        // Determine data elements
        var childElements = root.Elements().ToList();
        var elementGroups = childElements.GroupBy(e => e.Name.LocalName).ToList();

        if (elementGroups.Count == 1 && childElements.Count > 1)
        {
            // Repeating elements
            int count = 0;
            foreach (var element in childElements)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (maxRows.HasValue && count >= maxRows.Value)
                    break;

                results.Add(ConvertElementToDictionary(element));
                count++;
            }
        }
        else
        {
            // Single root element
            results.Add(ConvertElementToDictionary(root));
        }

        return Task.FromResult(results);
    }

    /// <summary>
    /// Sample properties from multiple XML elements
    /// </summary>
    private List<XmlPropertyInfo> SamplePropertiesFromElements(List<XElement> elements, int maxSampleSize = 100)
    {
        var propertyMap = new Dictionary<string, XmlPropertyInfo>();
        int sampleCount = 0;

        foreach (var element in elements)
        {
            if (sampleCount >= maxSampleSize)
                break;

            // Process attributes
            foreach (var attr in element.Attributes())
            {
                var name = $"@{attr.Name.LocalName}";
                if (!propertyMap.ContainsKey(name))
                {
                    propertyMap[name] = new XmlPropertyInfo
                    {
                        Name = attr.Name.LocalName,
                        InferredType = InferTypeFromString(attr.Value),
                        IsNullable = string.IsNullOrEmpty(attr.Value),
                        IsAttribute = true,
                        IsComplex = false,
                        IsRepeating = false
                    };
                }
            }

            // Process child elements
            foreach (var child in element.Elements())
            {
                var name = child.Name.LocalName;
                if (!propertyMap.ContainsKey(name))
                {
                    var isComplex = child.HasElements;
                    propertyMap[name] = new XmlPropertyInfo
                    {
                        Name = name,
                        InferredType = isComplex ? GenericDataTypes.Xml.Id : InferTypeFromString(child.Value),
                        IsNullable = string.IsNullOrEmpty(child.Value) && !child.HasElements,
                        IsAttribute = false,
                        IsComplex = isComplex,
                        IsRepeating = false
                    };
                }
                else if (string.IsNullOrEmpty(child.Value) && !child.HasElements)
                {
                    propertyMap[name].IsNullable = true;
                }
            }

            sampleCount++;
        }

        return propertyMap.Values.ToList();
    }

    /// <summary>
    /// Get properties from a single XML element
    /// </summary>
    private List<XmlPropertyInfo> GetPropertiesFromElement(XElement element)
    {
        var properties = new List<XmlPropertyInfo>();

        // Process attributes
        foreach (var attr in element.Attributes())
        {
            properties.Add(new XmlPropertyInfo
            {
                Name = attr.Name.LocalName,
                InferredType = InferTypeFromString(attr.Value),
                IsNullable = string.IsNullOrEmpty(attr.Value),
                IsAttribute = true,
                IsComplex = false,
                IsRepeating = false
            });
        }

        // Process child elements
        var childGroups = element.Elements().GroupBy(e => e.Name.LocalName);
        foreach (var group in childGroups)
        {
            var firstChild = group.First();
            var isComplex = firstChild.HasElements;
            var isRepeating = group.Count() > 1;

            properties.Add(new XmlPropertyInfo
            {
                Name = group.Key,
                InferredType = isComplex ? GenericDataTypes.Xml.Id : InferTypeFromString(firstChild.Value),
                IsNullable = string.IsNullOrEmpty(firstChild.Value) && !isComplex,
                IsAttribute = false,
                IsComplex = isComplex,
                IsRepeating = isRepeating
            });
        }

        return properties;
    }

    /// <summary>
    /// Extract column artifacts from multiple XML elements
    /// </summary>
    private void ExtractColumnArtifactsFromElements(TableArtifact table, List<XElement> elements, int maxSampleSize = 100)
    {
        var propertyUnion = new Dictionary<string, XmlPropertyUnionInfo>();
        int sampleCount = 0;

        foreach (var element in elements)
        {
            if (sampleCount >= maxSampleSize)
                break;

            // Process attributes
            foreach (var attr in element.Attributes())
            {
                var name = $"@{attr.Name.LocalName}";
                if (!propertyUnion.TryGetValue(name, out var unionInfo))
                {
                    unionInfo = new XmlPropertyUnionInfo
                    {
                        Name = attr.Name.LocalName,
                        InferredType = InferTypeFromString(attr.Value),
                        IsNullable = string.IsNullOrEmpty(attr.Value),
                        IsAttribute = true,
                        IsComplex = false,
                        IsRepeating = false
                    };
                    propertyUnion[name] = unionInfo;
                }
                else if (string.IsNullOrEmpty(attr.Value))
                {
                    unionInfo.IsNullable = true;
                }
            }

            // Process child elements
            foreach (var child in element.Elements())
            {
                var name = child.Name.LocalName;
                if (!propertyUnion.TryGetValue(name, out var unionInfo))
                {
                    var isComplex = child.HasElements || child.HasAttributes;
                    unionInfo = new XmlPropertyUnionInfo
                    {
                        Name = name,
                        InferredType = isComplex ? GenericDataTypes.Xml.Id : InferTypeFromString(child.Value),
                        IsNullable = string.IsNullOrEmpty(child.Value) && !isComplex,
                        IsAttribute = false,
                        IsComplex = isComplex,
                        IsRepeating = false,
                        SampleElements = new List<XElement>()
                    };
                    propertyUnion[name] = unionInfo;
                }
                else
                {
                    if (string.IsNullOrEmpty(child.Value) && !child.HasElements)
                    {
                        unionInfo.IsNullable = true;
                    }
                    if (child.HasElements)
                    {
                        unionInfo.IsComplex = true;
                        unionInfo.InferredType = GenericDataTypes.Xml.Id;
                    }
                }

                // Collect samples for nested extraction
                if (unionInfo.SampleElements.Count < 10 && (child.HasElements || child.HasAttributes))
                {
                    unionInfo.SampleElements.Add(child);
                }
            }

            sampleCount++;
        }

        // Create column artifacts
        int ordinal = 1;
        foreach (var unionInfo in propertyUnion.Values)
        {
            var column = new ColumnArtifact(unionInfo.Name, unionInfo.InferredType, true) // All from array are nullable
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

            table.AddChild(column);

            // Handle nested structure
            if (unionInfo.IsComplex && unionInfo.SampleElements.Count > 0)
            {
                var subTable = new TableArtifact(unionInfo.Name, string.Empty);
                ExtractColumnArtifactsFromElements(subTable, unionInfo.SampleElements);
                if (subTable.Children.Any())
                {
                    column.AddChild(subTable);
                }
            }
        }
    }

    /// <summary>
    /// Extract column artifacts from a single XML element
    /// </summary>
    private void ExtractColumnArtifactsFromElement(TableArtifact table, XElement element)
    {
        int ordinal = 1;

        // Process attributes
        foreach (var attr in element.Attributes())
        {
            var column = new ColumnArtifact(attr.Name.LocalName, InferTypeFromString(attr.Value), string.IsNullOrEmpty(attr.Value))
            {
                OrdinalPosition = ordinal++
            };

            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = attr.Name.LocalName,
                OriginalDataType = column.DataType,
                OriginalOrdinalPosition = column.OrdinalPosition,
                OriginalIsNullable = column.IsNullable
            });

            table.AddChild(column);
        }

        // Process child elements
        var childGroups = element.Elements().GroupBy(e => e.Name.LocalName);
        foreach (var group in childGroups)
        {
            var firstChild = group.First();
            var isComplex = firstChild.HasElements;
            var isRepeating = group.Count() > 1;

            var column = new ColumnArtifact(
                group.Key,
                isComplex ? GenericDataTypes.Xml.Id : InferTypeFromString(firstChild.Value),
                string.IsNullOrEmpty(firstChild.Value) && !isComplex)
            {
                OrdinalPosition = ordinal++
            };

            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = group.Key,
                OriginalDataType = column.DataType,
                OriginalOrdinalPosition = column.OrdinalPosition,
                OriginalIsNullable = column.IsNullable
            });

            table.AddChild(column);

            // Handle nested structure
            if (isComplex)
            {
                var subTable = new TableArtifact(group.Key, string.Empty);
                if (isRepeating)
                {
                    ExtractColumnArtifactsFromElements(subTable, group.ToList());
                }
                else
                {
                    ExtractColumnArtifactsFromElement(subTable, firstChild);
                }

                if (subTable.Children.Any())
                {
                    column.AddChild(subTable);
                }
            }
        }
    }

    /// <summary>
    /// Infer data type from a string value
    /// </summary>
    private string InferTypeFromString(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return GenericDataTypes.VarChar.Id;

        // Try boolean
        if (bool.TryParse(value, out _))
            return GenericDataTypes.Boolean.Id;

        // Try integer
        if (long.TryParse(value, out _))
            return GenericDataTypes.BigInt.Id;

        // Try decimal
        if (decimal.TryParse(value, out _))
            return GenericDataTypes.Decimal.Id;

        // Try date/time
        if (DateTime.TryParse(value, out _))
            return GenericDataTypes.DateTime.Id;

        // Try GUID
        if (Guid.TryParse(value, out _))
            return GenericDataTypes.Guid.Id;

        return GenericDataTypes.VarChar.Id;
    }

    /// <summary>
    /// Convert an XML element to a dictionary
    /// </summary>
    private Dictionary<string, object?> ConvertElementToDictionary(XElement element)
    {
        var dict = new Dictionary<string, object?>();

        // Add attributes
        foreach (var attr in element.Attributes())
        {
            dict[attr.Name.LocalName] = ConvertStringValue(attr.Value);
        }

        // Add child elements
        var childGroups = element.Elements().GroupBy(e => e.Name.LocalName);
        foreach (var group in childGroups)
        {
            if (group.Count() > 1)
            {
                // Multiple elements with same name - treat as array
                var arrayValue = group.Select(e => e.HasElements || e.HasAttributes ? ConvertElementToDictionary(e) : (object?)ConvertStringValue(e.Value)).ToList();
                dict[group.Key] = arrayValue;
                
                // Also add a singular property name for convenience in templates (pluralization)
                // This allows both subregion.Country and subregion.Countries to work
                var pluralKey = group.Key + "s";
                if (!dict.ContainsKey(pluralKey))
                {
                    dict[pluralKey] = arrayValue;
                }
            }
            else
            {
                var child = group.First();
                if (child.HasElements)
                {
                    // Check if this single element has repeating children
                    // If so, it might be better represented as a single object
                    dict[group.Key] = ConvertElementToDictionary(child);
                }
                else
                {
                    dict[group.Key] = ConvertStringValue(child.Value);
                }
            }
        }

        return dict;
    }

    /// <summary>
    /// Convert a string value to the appropriate .NET type
    /// </summary>
    private object? ConvertStringValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        // Try boolean
        if (bool.TryParse(value, out var boolVal))
            return boolVal;

        // Try long
        if (long.TryParse(value, out var longVal))
            return longVal;

        // Try decimal
        if (decimal.TryParse(value, out var decVal))
            return decVal;

        return value;
    }
}
