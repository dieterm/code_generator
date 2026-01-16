using CodeGenerator.Core.Templates;
using PlantUml.Net;

namespace CodeGenerator.TemplateEngines.PlantUML;

/// <summary>
/// Instance of a PlantUML template with parameters
/// </summary>
public class PlantUmlTemplateInstance : ITemplateInstance
{
    private readonly PlantUmlTemplate _template;

    public PlantUmlTemplateInstance(PlantUmlTemplate template)
    {
        _template = template ?? throw new ArgumentNullException(nameof(template));
    }

    public ITemplate Template => _template;

    /// <summary>
    /// Output file name for the generated image (e.g., "diagram.png")
    /// </summary>
    public string? OutputFileName { get; set; }

    /// <summary>
    /// Output format for the generated image (default: PNG)
    /// </summary>
    public OutputFormat OutputFormat { get; set; } = OutputFormat.Png;

    /// <summary>
    /// Additional parameters for the template
    /// </summary>
    public Dictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>();

    public void SetParameter(string key, object? value)
    {
        if (key == nameof(OutputFileName))
        {
            if (value is string outputFileName)
            {
                OutputFileName = outputFileName;
                return;
            }
            throw new ArgumentException($"Value for parameter '{key}' must be of type string.", nameof(value));
        }

        if (key == nameof(OutputFormat))
        {
            if (value is OutputFormat format)
            {
                OutputFormat = format;
                return;
            }
            throw new ArgumentException($"Value for parameter '{key}' must be of type {nameof(OutputFormat)}.", nameof(value));
        }

        Parameters[key] = value;
    }
}
