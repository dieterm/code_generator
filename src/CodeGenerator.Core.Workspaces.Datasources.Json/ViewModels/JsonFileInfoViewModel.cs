using CodeGenerator.Core.Workspaces.Datasources.Json.Services;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Core.Workspaces.Datasources.Json.ViewModels;

/// <summary>
/// ViewModel for JSON file info display
/// </summary>
public class JsonFileInfoViewModel : ViewModelBase
{
    public string TableName { get; set; } = string.Empty;
    public bool IsArray { get; set; }
    public int ItemCount { get; set; }
    public int PropertyCount { get; set; }
    public List<JsonPropertyInfoViewModel> Properties { get; set; } = new();
    public string DisplayName { get; set; } = string.Empty;

    public string TypeIcon => "table";

    public static JsonFileInfoViewModel FromJsonFileInfo(JsonFileInfo info)
    {
        return new JsonFileInfoViewModel
        {
            TableName = info.TableName,
            IsArray = info.IsArray,
            ItemCount = info.ItemCount,
            PropertyCount = info.PropertyCount,
            DisplayName = info.DisplayName,
            Properties = info.Properties.Select(p => new JsonPropertyInfoViewModel
            {
                Name = p.Name,
                InferredType = p.InferredType,
                IsNullable = p.IsNullable,
                IsObject = p.IsObject,
                IsArray = p.IsArray
            }).ToList()
        };
    }
}

/// <summary>
/// ViewModel for JSON property info
/// </summary>
public class JsonPropertyInfoViewModel
{
    public string Name { get; set; } = string.Empty;
    public string InferredType { get; set; } = "string";
    public bool IsNullable { get; set; }
    public bool IsObject { get; set; }
    public bool IsArray { get; set; }

    public string TypeDisplay => IsObject ? "object" : (IsArray ? "array" : InferredType);
}
