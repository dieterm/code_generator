namespace CodeGenerator.Domain.Templates;

using CodeGenerator.Domain.Artifacts;

/// <summary>
/// Interface for template engines
/// </summary>
public interface ITemplateEngine
{
    /// <summary>
    /// Name of the template engine
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Render a template with the given model
    /// </summary>
    string Render(string templateContent, object model);

    /// <summary>
    /// Render a template definition with the given model
    /// </summary>
    string Render(TemplateDefinition template, object model);

    /// <summary>
    /// Render a template instance to a file artifact
    /// </summary>
    Artifact RenderToArtifact(TemplateInstance instance);

    /// <summary>
    /// Validate template syntax
    /// </summary>
    TemplateValidationResult Validate(string templateContent);

    /// <summary>
    /// Load a template from file
    /// </summary>
    Task<TemplateDefinition> LoadTemplateAsync(string filePath, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of template validation
/// </summary>
public class TemplateValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; } = new();
    public List<string> Warnings { get; } = new();
}
