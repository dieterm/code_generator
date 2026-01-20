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

/// <summary>
/// Information about an XML element or attribute
/// </summary>
public class XmlPropertyInfo
{
    /// <summary>
    /// Name of the property (element or attribute name)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Inferred data type ID
    /// </summary>
    public string InferredType { get; set; } = string.Empty;

    /// <summary>
    /// Whether the property can be null/empty
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Whether this is an attribute (vs element)
    /// </summary>
    public bool IsAttribute { get; set; }

    /// <summary>
    /// Whether this contains child elements
    /// </summary>
    public bool IsComplex { get; set; }

    /// <summary>
    /// Whether this is a repeating element (array-like)
    /// </summary>
    public bool IsRepeating { get; set; }
}

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
