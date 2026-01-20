namespace CodeGenerator.Core.Workspaces.Datasources.Xml.Services;

/// <summary>
/// Information about an XML file structure
/// </summary>
public class XmlFileInfo
{
    /// <summary>
    /// Name of the file
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Default table name (derived from root element or file name)
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the root element
    /// </summary>
    public string RootElementName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the root contains repeating child elements (like an array)
    /// </summary>
    public bool HasRepeatingElements { get; set; }

    /// <summary>
    /// Name of the repeating child element (if applicable)
    /// </summary>
    public string? RepeatingElementName { get; set; }

    /// <summary>
    /// Number of repeating elements
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Number of detected properties/elements
    /// </summary>
    public int PropertyCount { get; set; }

    /// <summary>
    /// Detected properties/elements
    /// </summary>
    public List<XmlPropertyInfo> Properties { get; set; } = new();
}
