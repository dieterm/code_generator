using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Events;

/// <summary>
/// Event args for CreatedProject event
/// </summary>
public class CreatedProjectEventArgs : GeneratorEventArgs
{
    public GeneratedProject Project { get; }
    public IReadOnlyList<GeneratedFile> CreatedFiles { get; }

    public CreatedProjectEventArgs(DomainSchema schema, DomainContext context, GeneratedProject project, IReadOnlyList<GeneratedFile> createdFiles)
        : base(schema, context)
    {
        Project = project ?? throw new ArgumentNullException(nameof(project));
        CreatedFiles = createdFiles ?? throw new ArgumentNullException(nameof(createdFiles));
    }
}
