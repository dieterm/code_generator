namespace CodeGenerator.Core.Generators;

/// <summary>
/// Phases of the code generation process
/// </summary>
public enum GenerationPhase
{
    /// <summary>
    /// Initializing and parsing schema
    /// </summary>
    Initializing,

    /// <summary>
    /// Creating solution structure
    /// </summary>
    CreatingSolution,

    /// <summary>
    /// Creating projects
    /// </summary>
    CreatingProjects,

    /// <summary>
    /// Adding NuGet packages
    /// </summary>
    AddingPackages,

    /// <summary>
    /// Adding project references
    /// </summary>
    AddingReferences,

    /// <summary>
    /// Generating files
    /// </summary>
    GeneratingFiles,

    /// <summary>
    /// Running generators
    /// </summary>
    RunningGenerators,

    /// <summary>
    /// Finalizing
    /// </summary>
    Finalizing,

    /// <summary>
    /// Completed
    /// </summary>
    Completed
}
