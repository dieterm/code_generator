using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Events;

/// <summary>
/// Event args for CreatedFile event
/// </summary>
public class CreatedFileEventArgs : GeneratorEventArgs
{
    public GeneratedFile File { get; }
    public GeneratedProject? Project { get; }

    public CreatedFileEventArgs(DomainSchema schema, DomainContext context, GeneratedFile file, GeneratedProject? project = null)
        : base(schema, context)
    {
        File = file ?? throw new ArgumentNullException(nameof(file));
        Project = project;
    }
}
