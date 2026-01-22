using CodeGenerator.Core.Workspaces.Datasources.Yaml.Services;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Core.Workspaces.Datasources.Yaml.ViewModels;

/// <summary>
/// ViewModel for YAML file info display
/// </summary>
public class YamlFileInfoViewModel : ViewModelBase
{
    public string TableName { get; set; } = string.Empty;
    public bool IsSequence { get; set; }
    public int ItemCount { get; set; }
    public int PropertyCount { get; set; }
    public List<YamlPropertyInfoViewModel> Properties { get; set; } = new();
    public string DisplayName { get; set; } = string.Empty;

    public string TypeIcon => "table";

    public static YamlFileInfoViewModel FromYamlFileInfo(YamlFileInfo info)
    {
        return new YamlFileInfoViewModel
        {
            TableName = info.TableName,
            IsSequence = info.IsSequence,
            ItemCount = info.ItemCount,
            PropertyCount = info.PropertyCount,
            DisplayName = info.DisplayName,
            Properties = info.Properties.Select(p => new YamlPropertyInfoViewModel
            {
                Name = p.Name,
                InferredType = p.InferredType,
                IsNullable = p.IsNullable,
                IsMapping = p.IsMapping,
                IsSequence = p.IsSequence
            }).ToList()
        };
    }
}

/// <summary>
/// ViewModel for YAML property info
/// </summary>
public class YamlPropertyInfoViewModel
{
    public string Name { get; set; } = string.Empty;
    public string InferredType { get; set; } = "string";
    public bool IsNullable { get; set; }
    public bool IsMapping { get; set; }
    public bool IsSequence { get; set; }

    public string TypeDisplay => IsMapping ? "mapping" : (IsSequence ? "sequence" : InferredType);
}
