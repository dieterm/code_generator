namespace CodeGenerator.Core.Workspaces.Datasources.Xml.Services;

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
