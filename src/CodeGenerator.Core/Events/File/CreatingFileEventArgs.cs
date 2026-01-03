using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Events;

/// <summary>
/// Event args for CreatingFile event
/// </summary>
public class CreatingFileEventArgs : GeneratorEventArgs
{
    public FileRegistration File { get; }
    
    /// <summary>
    /// The project this file belongs to (if any)
    /// </summary>
    public GeneratedProject? Project { get; }
    
    /// <summary>
    /// Template being used (if any)
    /// </summary>
    public string? TemplateName { get; }
    
    /// <summary>
    /// Set to true to cancel file creation
    /// </summary>
    public bool Cancel { get; set; }
    
    /// <summary>
    /// Reason for cancellation
    /// </summary>
    public string? CancelReason { get; set; }

    public CreatingFileEventArgs(DomainSchema schema, DomainContext context, FileRegistration file, GeneratedProject? project = null, string? templateName = null)
        : base(schema, context)
    {
        File = file ?? throw new ArgumentNullException(nameof(file));
        Project = project;
        TemplateName = templateName;
    }
}
