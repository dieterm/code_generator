using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;

namespace CodeGenerator.Core.Interfaces;

/// <summary>
/// Interface for the template engine
/// </summary>
public interface ITemplateEngine
{
    /// <summary>
    /// Render a template with the given model
    /// </summary>
    Task<string> RenderAsync(string templateContent, object model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Render a template from file
    /// </summary>
    Task<string> RenderFileAsync(string templatePath, object model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compile and cache a template
    /// </summary>
    void CompileTemplate(string templateId, string templateContent);

    /// <summary>
    /// Render a pre-compiled template
    /// </summary>
    Task<string> RenderCompiledAsync(string templateId, object model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Register custom functions
    /// </summary>
    void RegisterFunction(string name, Delegate function);
}
