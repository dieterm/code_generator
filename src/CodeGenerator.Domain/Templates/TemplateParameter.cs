namespace CodeGenerator.Domain.Templates;

/// <summary>
/// Represents a parameter for a template
/// </summary>
public class TemplateParameter
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Parameter type
    /// </summary>
    public Type ParameterType { get; set; } = typeof(string);

    /// <summary>
    /// Default value for the parameter
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Whether the parameter is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Description of the parameter
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Display name for UI
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Allowed values (for enum-like parameters)
    /// </summary>
    public List<object>? AllowedValues { get; set; }
}
