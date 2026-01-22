using System.Text.Json;

namespace CodeGenerator.Core.Templates;

/// <summary>
/// Defines metadata for a template including its parameters
/// Serialized to JSON in a .def file alongside the template file
/// </summary>
public class TemplateDefinition
{
    /// <summary>
    /// Unique identifier for the template
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable name of the template
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Description of what this template generates
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Category for grouping templates (e.g., "SQL", "CSharp", "JavaScript")
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Tags for searching/filtering templates
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Version of the template
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Author of the template
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// List of parameters required by this template
    /// </summary>
    public List<TemplateParameter> Parameters { get; set; } = new();

    /// <summary>
    /// File extension pattern for the definition file
    /// </summary>
    public const string DefinitionFileExtension = ".def";

    /// <summary>
    /// Gets the definition file path for a template file
    /// </summary>
    public static string GetDefinitionFilePath(string templateFilePath)
    {
        return templateFilePath + DefinitionFileExtension;
    }

    /// <summary>
    /// Load a template definition from a .def file
    /// </summary>
    public static TemplateDefinition? LoadFromFile(string definitionFilePath)
    {
        if (!File.Exists(definitionFilePath))
            return null;

        var json = File.ReadAllText(definitionFilePath);
        return JsonSerializer.Deserialize<TemplateDefinition>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    /// <summary>
    /// Load a template definition for a template file (looks for .def file)
    /// </summary>
    public static TemplateDefinition? LoadForTemplate(string templateFilePath)
    {
        var defPath = GetDefinitionFilePath(templateFilePath);
        return LoadFromFile(defPath);
    }

    /// <summary>
    /// Save this template definition to a .def file
    /// </summary>
    public void SaveToFile(string definitionFilePath)
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        File.WriteAllText(definitionFilePath, json);
    }

    /// <summary>
    /// Save this template definition alongside a template file
    /// </summary>
    public void SaveForTemplate(string templateFilePath)
    {
        var defPath = GetDefinitionFilePath(templateFilePath);
        SaveToFile(defPath);
    }
    public void SaveForTemplate(TemplateArtifact template)
    {
        var defPath = GetDefinitionFilePath(template.FilePath);
        SaveToFile(defPath);
        template.RenameTemplate(TemplateId);
    }

    public void RenameDefinitionFile(string oldTemplateFilePath, string newTemplateFilePath)
    {
        if(string.IsNullOrWhiteSpace(oldTemplateFilePath))
            throw new ArgumentNullException(nameof(oldTemplateFilePath));
        if(string.IsNullOrWhiteSpace(newTemplateFilePath))
            throw new ArgumentNullException(nameof(newTemplateFilePath));

        var oldDefinitionFilePath = GetDefinitionFilePath(oldTemplateFilePath);
        var newDefinitionFilePath = GetDefinitionFilePath(newTemplateFilePath);

        if (File.Exists(oldDefinitionFilePath))
        {
            File.Move(oldDefinitionFilePath, newDefinitionFilePath);
        } 
        else
        {
            SaveToFile(newDefinitionFilePath);
        }
    }

    /// <summary>
    /// Creates a default/empty definition for a template file
    /// </summary>
    public static TemplateDefinition CreateDefault(string templateFilePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(templateFilePath);
        return new TemplateDefinition
        {
            TemplateId = fileName,
            DisplayName = fileName,
            Description = $"Template: {fileName}",
            Parameters = new List<TemplateParameter>()
        };
    }
}
