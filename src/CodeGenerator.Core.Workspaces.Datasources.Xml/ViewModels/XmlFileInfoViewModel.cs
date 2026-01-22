using CodeGenerator.Core.Workspaces.Datasources.Xml.Services;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Core.Workspaces.Datasources.Xml.ViewModels;

/// <summary>
/// ViewModel for XML file info display
/// </summary>
public class XmlFileInfoViewModel : ViewModelBase
{
    public string TableName { get; set; } = string.Empty;
    public string RootElementName { get; set; } = string.Empty;
    public bool HasRepeatingElements { get; set; }
    public string? RepeatingElementName { get; set; }
    public int ItemCount { get; set; }
    public int PropertyCount { get; set; }
    public List<XmlPropertyInfoViewModel> Properties { get; set; } = new();
    public string DisplayName => HasRepeatingElements
        ? $"{RepeatingElementName} ({ItemCount} items, {PropertyCount} properties)"
        : $"{RootElementName} ({PropertyCount} properties)";

    public string TypeIcon => "table";

    public static XmlFileInfoViewModel FromXmlFileInfo(XmlFileInfo info)
    {
        return new XmlFileInfoViewModel
        {
            TableName = info.TableName,
            RootElementName = info.RootElementName,
            HasRepeatingElements = info.HasRepeatingElements,
            RepeatingElementName = info.RepeatingElementName,
            ItemCount = info.ItemCount,
            PropertyCount = info.PropertyCount,
            Properties = info.Properties.Select(p => new XmlPropertyInfoViewModel
            {
                Name = p.Name,
                InferredType = p.InferredType,
                IsNullable = p.IsNullable,
                IsAttribute = p.IsAttribute,
                IsComplex = p.IsComplex,
                IsRepeating = p.IsRepeating
            }).ToList()
        };
    }
}

/// <summary>
/// ViewModel for XML property info
/// </summary>
public class XmlPropertyInfoViewModel
{
    public string Name { get; set; } = string.Empty;
    public string InferredType { get; set; } = "string";
    public bool IsNullable { get; set; }
    public bool IsAttribute { get; set; }
    public bool IsComplex { get; set; }
    public bool IsRepeating { get; set; }

    public string TypeDisplay
    {
        get
        {
            var prefix = IsAttribute ? "@" : "";
            var suffix = IsRepeating ? "[]" : "";
            var type = IsComplex ? "object" : InferredType;
            return $"{prefix}{type}{suffix}";
        }
    }
}
