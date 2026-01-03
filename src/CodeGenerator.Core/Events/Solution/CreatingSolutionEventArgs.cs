using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Events;

/// <summary>
/// Event args for CreatingSolution event
/// </summary>
public class CreatingSolutionEventArgs : GeneratorEventArgs
{
    public SolutionInfo Solution { get; }
    public GeneratorSettings Settings { get; }

    /// <summary>
    /// Bucket where generators can register projects to be created
    /// </summary>
    public List<ProjectRegistration> ProjectRegistrations { get; } = new();

    public CreatingSolutionEventArgs(DomainSchema schema, DomainContext context, SolutionInfo solution, GeneratorSettings settings)
        : base(schema, context)
    {
        Solution = solution ?? throw new ArgumentNullException(nameof(solution));
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
}
