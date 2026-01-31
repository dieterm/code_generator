using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Templates.Settings;

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
    
    TemplateEngineSettingsDescription SettingsDescription { get; }
    void Initialize();

    bool SupportsTemplate(ITemplate template);
    bool SupportsTemplateType(TemplateType templateType);

    ITemplateInstance CreateTemplateInstance(ITemplate template);
    /// <summary>
    /// Render a template with the given model
    /// </summary>
    Task<TemplateOutput> RenderAsync(ITemplateInstance templateInstance, CancellationToken cancellationToken = default);
}
