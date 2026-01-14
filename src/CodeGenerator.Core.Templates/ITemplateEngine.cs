using CodeGenerator.Core.Templates;

namespace CodeGenerator.Core.Interfaces;

/// <summary>
/// Interface for the template engine
/// </summary>
public interface ITemplateEngine
{
    /// <summary>
    /// Unique identifier of the template engine.
    /// Use a short lowercase string without spaces, eg. 'scriban_template_engine'.
    /// </summary>
    string Id { get; }
    /// <summary>
    /// Display name used in logs and user interface
    /// </summary>
    string DisplayName { get; }
    bool SupportsTemplatePath(string fileOrFolderName);
    /// <summary>
    /// File extensions supported by this template engine (without dot) eg. "scriban", "tt"<br />
    /// Used to identify template files for this engine.
    /// For folder-based templates, should throw exception or return false.
    /// </summary>
    bool SupportsTemplateFileExtension(string fileExtension);
    bool SupportsTemplate(ITemplate template);
    bool SupportsTemplateType(TemplateType templateType);
    ITemplate CreateTemplateFromFile(string filePath);
    ITemplateInstance CreateTemplateInstance(ITemplate template);
    /// <summary>
    /// Render a template with the given model
    /// </summary>
    Task<TemplateOutput> RenderAsync(ITemplateInstance templateInstance, CancellationToken cancellationToken = default);
}
