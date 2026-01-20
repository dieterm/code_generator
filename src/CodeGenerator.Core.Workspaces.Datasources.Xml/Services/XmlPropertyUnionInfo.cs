namespace CodeGenerator.Core.Workspaces.Datasources.Xml.Services;

/// <summary>
/// Internal class for tracking property information during union building
/// </summary>
internal class XmlPropertyUnionInfo
{
    public string Name { get; set; } = string.Empty;
    public string InferredType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsAttribute { get; set; }
    public bool IsComplex { get; set; }
    public bool IsRepeating { get; set; }
    public List<System.Xml.Linq.XElement> SampleElements { get; set; } = new();
}
