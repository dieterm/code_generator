using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Events;

/// <summary>
/// Event args for CreatedSolution event
/// </summary>
public class CreatedSolutionEventArgs : GeneratorEventArgs
{
    public SolutionInfo Solution { get; }
    
    /// <summary>
    /// List of projects that were created
    /// </summary>
    public IReadOnlyList<GeneratedProject> CreatedProjects { get; }

    public CreatedSolutionEventArgs(DomainSchema schema, DomainContext context, SolutionInfo solution, IReadOnlyList<GeneratedProject> createdProjects)
        : base(schema, context)
    {
        Solution = solution ?? throw new ArgumentNullException(nameof(solution));
        CreatedProjects = createdProjects ?? throw new ArgumentNullException(nameof(createdProjects));
    }
}
