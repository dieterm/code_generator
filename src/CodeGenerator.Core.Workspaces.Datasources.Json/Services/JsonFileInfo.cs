using System.Text.Json;

namespace CodeGenerator.Core.Workspaces.Datasources.Json.Services;

/// <summary>
/// Information about a JSON file structure
/// </summary>
public class JsonFileInfo
{
    public string FileName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public bool IsArray { get; set; }
    public int ItemCount { get; set; }
    public int PropertyCount { get; set; }
    public List<JsonPropertyInfo> Properties { get; set; } = new();

    public string DisplayName => IsArray 
        ? $"{TableName} (Array with {ItemCount} items, {PropertyCount} properties)" 
        : $"{TableName} (Object with {PropertyCount} properties)";
}

/// <summary>
/// Information about a JSON property
/// </summary>
public class JsonPropertyInfo
{
    public string Name { get; set; } = string.Empty;
    public string InferredType { get; set; } = "string";
    public bool IsNullable { get; set; } = true;
    public bool IsObject { get; set; }
    public bool IsArray { get; set; }
}

/// <summary>
/// Extended property info used during union extraction to hold sample values
/// </summary>
internal class JsonPropertyUnionInfo
{
    public string Name { get; set; } = string.Empty;
    public string InferredType { get; set; } = "string";
    public bool IsNullable { get; set; } = true;
    public bool IsObject { get; set; }
    public bool IsArray { get; set; }
    
    /// <summary>
    /// Sample values collected for nested extraction
    /// </summary>
    public List<JsonElement> SampleValues { get; set; } = new();
}
