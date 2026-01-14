namespace CodeGenerator.Core.Templates;

/// <summary>
/// Defines a template parameter that can be configured before rendering a template
/// </summary>
public class TemplateParameter
{
    /// <summary>
    /// Name of the parameter (used as key in template context)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of the parameter
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Fully qualified assembly type name of the parameter value type
    /// Examples: "System.String", "System.Int32", "System.Boolean"
    /// </summary>
    public string FullyQualifiedAssemblyTypeName { get; set; } = "System.String";

    /// <summary>
    /// Whether this parameter is required for template rendering
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Default value for this parameter (serialized as string)
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Optional list of allowed values for this parameter (for dropdown/combobox selection)
    /// </summary>
    public List<string>? AllowedValues { get; set; }

    /// <summary>
    /// Display label for the parameter (if not set, Name is used)
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Tooltip text for the parameter input control
    /// </summary>
    public string? Tooltip { get; set; }

    /// <summary>
    /// Order in which this parameter should be displayed (lower values first)
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Optional validation regex pattern for string values
    /// </summary>
    public string? ValidationPattern { get; set; }

    /// <summary>
    /// Optional minimum value (for numeric types)
    /// </summary>
    public string? MinValue { get; set; }

    /// <summary>
    /// Optional maximum value (for numeric types)
    /// </summary>
    public string? MaxValue { get; set; }

    /// <summary>
    /// Gets the display label (Label if set, otherwise Name)
    /// </summary>
    public string GetDisplayLabel() => !string.IsNullOrEmpty(Label) ? Label : Name;

    /// <summary>
    /// Gets the resolved Type from FullyQualifiedAssemblyTypeName
    /// </summary>
    public Type? GetParameterType()
    {
        if (string.IsNullOrEmpty(FullyQualifiedAssemblyTypeName))
            return typeof(string);

        return Type.GetType(FullyQualifiedAssemblyTypeName);
    }
}
