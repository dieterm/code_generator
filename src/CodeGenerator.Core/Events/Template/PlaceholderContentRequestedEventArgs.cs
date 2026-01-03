using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Events;

/// <summary>
/// Event args for PlaceholderContentRequested event
/// </summary>
public class PlaceholderContentRequestedEventArgs : GeneratorEventArgs
{
    /// <summary>
    /// Name of the placeholder
    /// </summary>
    public string PlaceholderName { get; }
    
    /// <summary>
    /// The file being generated
    /// </summary>
    public FileRegistration File { get; }
    
    /// <summary>
    /// Template being used
    /// </summary>
    public string? TemplateName { get; }
    
    /// <summary>
    /// Entity being processed (if applicable)
    /// </summary>
    public EntityModel? Entity { get; }
    
    /// <summary>
    /// Content contributions from subscribers
    /// </summary>
    public List<PlaceholderContent> ContentContributions { get; } = new();

    public PlaceholderContentRequestedEventArgs(
        DomainSchema schema, 
        DomainContext context, 
        string placeholderName, 
        FileRegistration file,
        string? templateName = null,
        EntityModel? entity = null)
        : base(schema, context)
    {
        PlaceholderName = placeholderName ?? throw new ArgumentNullException(nameof(placeholderName));
        File = file ?? throw new ArgumentNullException(nameof(file));
        TemplateName = templateName;
        Entity = entity;
    }
    
    /// <summary>
    /// Get combined content from all contributors, ordered by priority
    /// </summary>
    public string GetCombinedContent(string separator = "\n")
    {
        return string.Join(separator, ContentContributions
            .OrderBy(c => c.Priority)
            .Select(c => c.Content));
    }
}
