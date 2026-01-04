namespace CodeGenerator.Domain.Templates;

/// <summary>
/// Represents an instance of a template with parameter values
/// </summary>
public class TemplateInstance
{
    /// <summary>
    /// The template definition
    /// </summary>
    public TemplateDefinition Definition { get; }

    /// <summary>
    /// Parameter values for this instance
    /// </summary>
    public Dictionary<string, object> ParameterValues { get; }

    /// <summary>
    /// Output file name (after rendering)
    /// </summary>
    public string? OutputFileName { get; set; }

    /// <summary>
    /// Output directory (relative path)
    /// </summary>
    public string? OutputDirectory { get; set; }

    public TemplateInstance(TemplateDefinition definition, Dictionary<string, object> parameterValues)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        ParameterValues = parameterValues ?? new();
    }

    /// <summary>
    /// Set a parameter value
    /// </summary>
    public TemplateInstance SetParameter(string name, object value)
    {
        ParameterValues[name] = value;
        return this;
    }

    /// <summary>
    /// Get a parameter value
    /// </summary>
    public T? GetParameter<T>(string name)
    {
        if (ParameterValues.TryGetValue(name, out var value) && value is T typed)
            return typed;
        return default;
    }

    /// <summary>
    /// Check if all required parameters are set
    /// </summary>
    public bool AreRequiredParametersSet()
    {
        foreach (var param in Definition.Parameters.Where(p => p.IsRequired))
        {
            if (!ParameterValues.ContainsKey(param.Name))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Get missing required parameters
    /// </summary>
    public IEnumerable<TemplateParameter> GetMissingRequiredParameters()
    {
        return Definition.Parameters
            .Where(p => p.IsRequired && !ParameterValues.ContainsKey(p.Name));
    }
}
