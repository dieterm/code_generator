using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Events;

/// <summary>
/// Event args for CreatingProject event
/// </summary>
public class CreatingProjectEventArgs : GeneratorEventArgs
{
    public ProjectRegistration Project { get; }
    
    /// <summary>
    /// Bucket where generators can register files to be added to this project
    /// </summary>
    public List<FileRegistration> FileRegistrations { get; } = new();
    
    /// <summary>
    /// Additional NuGet packages to add
    /// </summary>
    public List<NuGetPackageInfo> AdditionalPackages { get; } = new();
    
    /// <summary>
    /// Additional project references
    /// </summary>
    public List<string> AdditionalProjectReferences { get; } = new();

    public CreatingProjectEventArgs(DomainSchema schema, DomainContext context, ProjectRegistration project)
        : base(schema, context)
    {
        Project = project ?? throw new ArgumentNullException(nameof(project));
    }
}
