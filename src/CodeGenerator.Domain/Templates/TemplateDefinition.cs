namespace CodeGenerator.Domain.Templates;

/// <summary>
/// Represents a template definition
/// </summary>
public class TemplateDefinition
{
    /// <summary>
    /// Unique identifier for the template
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the template
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the template
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Template content (the actual template text)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// File path if loaded from file
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Template engine to use (e.g., "scriban", "razor")
    /// </summary>
    public string Engine { get; set; } = "scriban";

    /// <summary>
    /// Output file extension
    /// </summary>
    public string OutputExtension { get; set; } = ".cs";

    /// <summary>
    /// Parameters for the template
    /// </summary>
    public List<TemplateParameter> Parameters { get; } = new();

    /// <summary>
    /// Category for organizing templates
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Tags for searching templates
    /// </summary>
    public List<string> Tags { get; } = new();

    /// <summary>
    /// Create an instance of this template with parameter values
    /// </summary>
    public TemplateInstance CreateInstance(Dictionary<string, object>? parameterValues = null)
    {
        return new TemplateInstance(this, parameterValues ?? new());
    }
}
