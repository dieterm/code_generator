using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;

namespace CodeGenerator.Core.Interfaces;

/// <summary>
/// Interface for project generation using MSBuild
/// </summary>
public interface IProjectGenerator
{
    /// <summary>
    /// Create a new project
    /// </summary>
    Task<GeneratedProject> CreateProjectAsync(string name, string directory, string projectType, string targetFramework, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a NuGet package to a project
    /// </summary>
    Task AddPackageAsync(string projectPath, string packageId, string version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a project reference
    /// </summary>
    Task AddProjectReferenceAsync(string projectPath, string referencePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a solution and add projects
    /// </summary>
    Task<string> CreateSolutionAsync(string name, string directory, IEnumerable<string> projectPaths, CancellationToken cancellationToken = default);

    /// <summary>
    /// Build a project
    /// </summary>
    Task<bool> BuildAsync(string projectPath, CancellationToken cancellationToken = default);
}
