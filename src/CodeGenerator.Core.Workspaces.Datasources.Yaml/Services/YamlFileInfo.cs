namespace CodeGenerator.Core.Workspaces.Datasources.Yaml.Services;

/// <summary>
/// Information about a YAML file structure
/// </summary>
public class YamlFileInfo
{
    public string FileName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public bool IsSequence { get; set; }
    public int ItemCount { get; set; }
    public int PropertyCount { get; set; }
    public List<YamlPropertyInfo> Properties { get; set; } = new();

    public string DisplayName => IsSequence 
        ? $"{TableName} (Sequence with {ItemCount} items, {PropertyCount} properties)" 
        : $"{TableName} (Mapping with {PropertyCount} properties)";
}

/// <summary>
/// Information about a YAML property
/// </summary>
public class YamlPropertyInfo
{
    public string Name { get; set; } = string.Empty;
    public string InferredType { get; set; } = "string";
    public bool IsNullable { get; set; } = true;
    public bool IsMapping { get; set; }
    public bool IsSequence { get; set; }
}

/// <summary>
/// Extended property info used during union extraction to hold sample values
/// </summary>
internal class YamlPropertyUnionInfo
{
    public string Name { get; set; } = string.Empty;
    public string InferredType { get; set; } = "string";
    public bool IsNullable { get; set; } = true;
    public bool IsMapping { get; set; }
    public bool IsSequence { get; set; }
    
    /// <summary>
    /// Sample values collected for nested extraction
    /// </summary>
    public List<object> SampleValues { get; set; } = new();
}
